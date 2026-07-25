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
    }
}