using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.Servicing
{
    [Table("Payments")]
    public class Payment
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? Mode { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Remarks { get; set; }
        public string? PaymentType { get; set; }
        public string? UpdatedBy { get; set; }
        [Computed]
        public string LoanNumber { get; set; } = string.Empty;

        [Computed]
        public string BorrowerName { get; set; } = string.Empty;

        [Computed]
        public decimal CurrentDueAmount { get; set; }

        public static async Task<Payment?> GetPaymentScreenDataAsync(
    IDatabaseConnection databaseConnection,
    Guid loanId)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
    SELECT
        l.LoanNumber,
        c.Name AS BorrowerName,

        (
            SELECT TOP 1
                (Principal + Interest)
            FROM RepaymentSchedules
            WHERE LoanId = @LoanId
              AND Status <> 'Paid'
            ORDER BY EmiNo
        ) AS CurrentDueAmount

    FROM Loans l
    INNER JOIN Customers c
        ON l.CustomerId = c.Id

    WHERE l.Id = @LoanId";

            return await connection.QueryFirstOrDefaultAsync<Payment>(
                sql,
                new
                {
                    LoanId = loanId
                });
        }
        public static async Task<List<Payment>> GetByLoanIdAsync(IDatabaseConnection databaseConnection, Guid loanId)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = "SELECT * FROM [dbo].[Payments] WHERE LoanId = @LoanId ORDER BY PaymentDate";
            var result = await connection.QueryAsync<Payment>(sql, new { LoanId = loanId });
            return result.ToList();
        }

        public static async Task RecordPaymentTransactionAsync(IDatabaseConnection databaseConnection, Payment payment)
        {
            using var connection = databaseConnection.GetConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();
            try
            {
                // 1. Insert Payment Record
                const string paymentSql = @"
                    INSERT INTO [dbo].[Payments] 
                    (Id, LoanId, PaymentDate, Amount, Mode, ReferenceNumber, Remarks, PaymentType, UpdatedBy)
                    VALUES (@Id, @LoanId, @PaymentDate, @Amount, @Mode, @ReferenceNumber, @Remarks, @PaymentType, @UpdatedBy)";

                await connection.ExecuteAsync(paymentSql, payment, transaction);

                // 2. Find the earliest pending schedule item for this loan and mark it as 'Paid'
                const string findScheduleSql = @"
                    SELECT TOP 1 Id FROM [dbo].[RepaymentSchedules] 
                    WHERE LoanId = @LoanId AND Status <> 'Paid' 
                    ORDER BY EmiNo ASC";

                var scheduleId = await connection.QueryFirstOrDefaultAsync<Guid?>(findScheduleSql, new { LoanId = payment.LoanId }, transaction);

                if (scheduleId.HasValue && scheduleId.Value != Guid.Empty)
                {
                    const string updateScheduleSql = "UPDATE [dbo].[RepaymentSchedules] SET Status = 'Paid', UpdatedBy = @UpdatedBy WHERE Id = @ScheduleId";
                    await connection.ExecuteAsync(updateScheduleSql, new { ScheduleId = scheduleId.Value, UpdatedBy = payment.UpdatedBy }, transaction);
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