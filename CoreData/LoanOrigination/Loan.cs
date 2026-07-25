using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.LoanOrigination
{
    [Table("Loans")]
    public class Loan
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string LoanNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int Tenure { get; set; }
        public string RepaymentFrequency { get; set; } = "Monthly";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Draft";
        public string? UpdatedBy { get; set; }

        [Computed]
        public string CustomerName { get; set; } = string.Empty;

        [Computed]
        public string ProductName { get; set; } = string.Empty;

        public static async Task<List<Loan>> GetAllWithDetailsAsync(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
                SELECT 
                    l.*,
                    c.Name AS CustomerName,
                    p.Name AS ProductName
                FROM [dbo].[Loans] l
                INNER JOIN [dbo].[Customers] c ON l.CustomerId = c.Id
                INNER JOIN [dbo].[LoanProducts] p ON l.ProductId = p.Id
                ORDER BY l.LoanNumber DESC";

            var result = await connection.QueryAsync<Loan>(sql);
            return result.ToList();
        }

        public static async Task OriginateLoanWithScheduleAsync(IDatabaseConnection databaseConnection, Loan loan)
        {
            using var connection = databaseConnection.GetConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();
            try
            {
                // 1. Insert Loan
                const string loanSql = @"
                    INSERT INTO [dbo].[Loans] 
                    (Id, LoanNumber, CustomerId, ProductId, Principal, InterestRate, Tenure, RepaymentFrequency, StartDate, EndDate, Status, UpdatedBy)
                    VALUES (@Id, @LoanNumber, @CustomerId, @ProductId, @Principal, @InterestRate, @Tenure, @RepaymentFrequency, @StartDate, @EndDate, @Status, @UpdatedBy)";

                await connection.ExecuteAsync(loanSql, loan, transaction);

                // 2. Generate Amortization Schedule
                decimal principal = loan.Principal;
                decimal annualRate = loan.InterestRate;
                int tenure = loan.Tenure;

                decimal monthlyRate = (annualRate / 100m) / 12m;
                decimal emi = 0;

                if (monthlyRate > 0)
                {
                    double compound = Math.Pow((double)(1 + monthlyRate), tenure);
                    emi = principal * monthlyRate * (decimal)compound / (decimal)(compound - 1);
                }
                else
                {
                    emi = principal / tenure;
                }

                decimal outstanding = principal;
                DateTime dueDate = loan.StartDate ?? DateTime.Today;

                for (int i = 1; i <= tenure; i++)
                {
                    dueDate = dueDate.AddMonths(1);
                    decimal interestPayment = outstanding * monthlyRate;
                    decimal principalPayment = emi - interestPayment;
                    outstanding -= principalPayment;
                    if (outstanding < 0) outstanding = 0;

                    const string scheduleSql = @"
                        INSERT INTO [dbo].[RepaymentSchedules] 
                        (Id, LoanId, EmiNo, DueDate, Principal, Interest, Outstanding, Status, UpdatedBy)
                        VALUES (@Id, @LoanId, @EmiNo, @DueDate, @Principal, @Interest, @Outstanding, @Status, @UpdatedBy)";

                    await connection.ExecuteAsync(scheduleSql, new
                    {
                        Id = Guid.NewGuid(),
                        LoanId = loan.Id,
                        EmiNo = i,
                        DueDate = dueDate,
                        Principal = Math.Round(principalPayment, 2),
                        Interest = Math.Round(interestPayment, 2),
                        Outstanding = Math.Round(outstanding, 2),
                        Status = "Pending",
                        UpdatedBy = loan.UpdatedBy
                    }, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public class DashboardMetrics
        {
            public int ActiveLoanCount { get; set; }
            public decimal ActiveLoanTotal { get; set; }

            public int OverdueAccountCount { get; set; }
            public decimal OverdueAmountTotal { get; set; }

            public decimal CollectionsToday { get; set; }

            public List<ProductDistributionDto> ProductDistribution { get; set; } = new();
            public List<DailyCollectionDto> Last7DaysCollections { get; set; } = new();

            public static async Task<DashboardMetrics> GetLiveMetricsAsync(IDatabaseConnection databaseConnection, DateTime today)
            {
                using var connection = databaseConnection.GetConnection();
                var metrics = new DashboardMetrics();

                // 1. Active Loans
                const string activeSql = @"
                SELECT 
                    COUNT(Id) AS ActiveLoanCount, 
                    ISNULL(SUM(Principal), 0) AS ActiveLoanTotal
                FROM [dbo].[Loans] 
                WHERE Status NOT IN ('Draft', 'Closed', 'Rejected')";

                var activeData = await connection.QueryFirstOrDefaultAsync<DashboardMetrics>(activeSql);
                if (activeData != null)
                {
                    metrics.ActiveLoanCount = activeData.ActiveLoanCount;
                    metrics.ActiveLoanTotal = activeData.ActiveLoanTotal;
                }

                // 2. Overdue Accounts (Pending schedules where DueDate is in the past)
                const string overdueSql = @"
                SELECT 
                    COUNT(DISTINCT LoanId) AS OverdueAccountCount, 
                    ISNULL(SUM(Principal + Interest), 0) AS OverdueAmountTotal
                FROM [dbo].[RepaymentSchedules]
                WHERE Status = 'Pending' AND DueDate < @Today";

                var overdueData = await connection.QueryFirstOrDefaultAsync<DashboardMetrics>(overdueSql, new { Today = today.Date });
                if (overdueData != null)
                {
                    metrics.OverdueAccountCount = overdueData.OverdueAccountCount;
                    metrics.OverdueAmountTotal = overdueData.OverdueAmountTotal;
                }

                // 3. Collections Today
                const string todayColSql = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM [dbo].[Payments]
                WHERE CAST(PaymentDate AS DATE) = CAST(@Today AS DATE)";

                metrics.CollectionsToday = await connection.QueryFirstOrDefaultAsync<decimal>(todayColSql, new { Today = today.Date });

                // 4. Product Distribution
                const string prodSql = @"
                SELECT 
                    p.Name AS ProductName, 
                    COUNT(l.Id) AS LoanCount
                FROM [dbo].[Loans] l
                INNER JOIN [dbo].[LoanProducts] p ON l.ProductId = p.Id
                WHERE l.Status NOT IN ('Draft', 'Rejected')
                GROUP BY p.Name";

                var prodDist = await connection.QueryAsync<ProductDistributionDto>(prodSql);
                metrics.ProductDistribution = prodDist.ToList();

                // 5. Last 7 Days Collections Trend
                var sevenDaysAgo = today.Date.AddDays(-6);
                const string trendSql = @"
                SELECT 
                    CAST(PaymentDate AS DATE) AS PaymentDate, 
                    ISNULL(SUM(Amount), 0) AS TotalAmount
                FROM [dbo].[Payments]
                WHERE PaymentDate >= @SevenDaysAgo
                GROUP BY CAST(PaymentDate AS DATE)";

                var trends = await connection.QueryAsync<DailyCollectionDto>(trendSql, new { SevenDaysAgo = sevenDaysAgo });

                // Fill in missing days with 0 for the chart
                for (int i = 0; i < 7; i++)
                {
                    var targetDate = sevenDaysAgo.AddDays(i);
                    var existing = trends.FirstOrDefault(t => t.PaymentDate.Date == targetDate.Date);
                    metrics.Last7DaysCollections.Add(existing ?? new DailyCollectionDto { PaymentDate = targetDate, TotalAmount = 0 });
                }

                return metrics;
            }
        }

        public class ProductDistributionDto
        {
            public string ProductName { get; set; } = string.Empty;
            public int LoanCount { get; set; }
            public double Percentage { get; set; }
        }

        public class DailyCollectionDto
        {
            public DateTime PaymentDate { get; set; }
            public decimal TotalAmount { get; set; }
            public int ChartHeight { get; set; }
        }
    }
}