using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.LoanOrigination
{
    [Table("Disbursements")]
    public class Disbursement
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal Principal { get; set; }
        public string BankAccount { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }

        public static async Task<List<Disbursement>> GetByLoanIdAsync(IDatabaseConnection databaseConnection, Guid loanId)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = "SELECT * FROM [dbo].[Disbursements] WHERE LoanId = @LoanId";
            var result = await connection.QueryAsync<Disbursement>(sql, new { LoanId = loanId });
            return result.ToList();
        }

        public static async Task InsertAsync(IDatabaseConnection databaseConnection, Disbursement disbursement)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
        INSERT INTO [dbo].[Disbursements]
        (
            Id,
            LoanId,
            Principal,
            BankAccount,
            TransactionDate,
            ReferenceNumber,
            UpdatedBy
        )
        VALUES
        (
            @Id,
            @LoanId,
            @Principal,
            @BankAccount,
            @TransactionDate,
            @ReferenceNumber,
            @UpdatedBy
        )";

            await connection.ExecuteAsync(sql, disbursement);
        }
    }
}