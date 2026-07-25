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
    }
}