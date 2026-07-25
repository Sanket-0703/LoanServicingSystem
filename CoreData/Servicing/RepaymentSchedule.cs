using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.Servicing
{
    [Table("RepaymentSchedules")]
    public class RepaymentSchedule
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public int EmiNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Outstanding { get; set; }
        public string Status { get; set; } = "Pending";
        public string? UpdatedBy { get; set; }

        public static async Task<List<RepaymentSchedule>> GetByLoanIdAsync(IDatabaseConnection databaseConnection, Guid loanId)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = "SELECT * FROM [dbo].[RepaymentSchedules] WHERE LoanId = @LoanId ORDER BY EmiNo";
            var result = await connection.QueryAsync<RepaymentSchedule>(sql, new { LoanId = loanId });
            return result.ToList();
        }

        public class CollectionsData
        {
            public decimal TotalOverduePortfolio { get; set; }
            public int Dpd30PlusAccounts { get; set; }
            public decimal RecoveredToday { get; set; }
            public List<DelinquentAccountDto> DelinquentAccounts { get; set; } = new();

            public static async Task<CollectionsData> GetCollectionsWorkspaceAsync(IDatabaseConnection databaseConnection, DateTime today)
            {
                using var connection = databaseConnection.GetConnection();
                var data = new CollectionsData();

                // 1. Fetch Delinquent Accounts & DPD
                const string delinquentSql = @"
                SELECT 
                    l.Id AS LoanId,
                    l.LoanNumber,
                    c.Name AS BorrowerName,
                    c.Id AS CustomerId,
                    DATEDIFF(day, MIN(rs.DueDate), @Today) AS DaysPastDue,
                    SUM(rs.Principal + rs.Interest) AS OverdueAmount,
                    0 AS PenaltyDue -- Defaulting to 0 until dynamic penalty accrual is implemented
                FROM [dbo].[RepaymentSchedules] rs
                INNER JOIN [dbo].[Loans] l ON rs.LoanId = l.Id
                INNER JOIN [dbo].[Customers] c ON l.CustomerId = c.Id
                WHERE rs.Status = 'Pending' AND rs.DueDate < @Today
                GROUP BY l.Id, l.LoanNumber, c.Name, c.Id
                ORDER BY DaysPastDue DESC";

                var accounts = await connection.QueryAsync<DelinquentAccountDto>(delinquentSql, new { Today = today.Date });
                data.DelinquentAccounts = accounts.ToList();

                // 2. Compute Portfolio Metrics
                data.TotalOverduePortfolio = data.DelinquentAccounts.Sum(a => a.OverdueAmount);
                data.Dpd30PlusAccounts = data.DelinquentAccounts.Count(a => a.DaysPastDue >= 30);

                // 3. Recovered Today (Payments received today)
                const string recoveredSql = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM [dbo].[Payments]
                WHERE CAST(PaymentDate AS DATE) = CAST(@Today AS DATE)";

                data.RecoveredToday = await connection.QueryFirstOrDefaultAsync<decimal>(recoveredSql, new { Today = today.Date });

                return data;
            }
        }

        public class DelinquentAccountDto
        {
            public Guid LoanId { get; set; }
            public string LoanNumber { get; set; } = string.Empty;
            public string BorrowerName { get; set; } = string.Empty;
            public Guid CustomerId { get; set; }
            public int DaysPastDue { get; set; }
            public decimal OverdueAmount { get; set; }
            public decimal PenaltyDue { get; set; }
        }
    }
}