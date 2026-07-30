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
        [Computed]
        public decimal TotalCollected { get; set; }

        [Computed]
        public DateTime? LastPaymentDate { get; set; }

        [Computed]
        public int TotalPayments { get; set; }

        public static async Task<Payment?> GetPaymentScreenDataAsync(IDatabaseConnection databaseConnection, Guid loanId)
        {
            try
            {
                using var connection = databaseConnection.GetConnection();

                const string sql = @"
   SELECT

    l.LoanNumber,

    c.Name AS BorrowerName,

    ISNULL
(
    (
        SELECT SUM(Principal + Interest)
        FROM RepaymentSchedules
        WHERE LoanId = @LoanId
          AND Status <> 'Paid'
          AND DueDate < CAST(GETDATE() AS DATE)
    ),
    0
) AS CurrentDueAmount,

    ISNULL(
    (
        SELECT SUM(Amount)
        FROM Payments
        WHERE LoanId = @LoanId
    ),0) AS TotalCollected,

    (
        SELECT MAX(PaymentDate)
        FROM Payments
        WHERE LoanId=@LoanId
    ) AS LastPaymentDate,

    (
        SELECT COUNT(*)
        FROM Payments
        WHERE LoanId=@LoanId
    ) AS TotalPayments

FROM Loans l

INNER JOIN Customers c
ON l.CustomerId=c.Id

WHERE l.Id=@LoanId";

                return await connection.QueryFirstOrDefaultAsync<Payment>(
                    sql,
                    new
                    {
                        LoanId = loanId
                    });
            }
            catch (Exception ex)
            {
                // Log the exact SQL error to the server console
                Console.WriteLine($"Error fetching payment screen data for Loan {loanId}: {ex.Message}");

                // Rethrow the exception so LoadDataAsync catches it and displays it
                throw;
            }
        }
        public static async Task<List<Payment>> GetByLoanIdAsync(IDatabaseConnection databaseConnection, Guid loanId)
        {
            try
            {
                using var connection = databaseConnection.GetConnection();
                const string sql = "SELECT * FROM [dbo].[Payments] WHERE LoanId = @LoanId ORDER BY PaymentDate";
                var result = await connection.QueryAsync<Payment>(sql, new { LoanId = loanId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                // Log the exact SQL error to the server console
                Console.WriteLine($"Error fetching payments for Loan {loanId}: {ex.Message}");

                // Rethrow the exception so LoadDataAsync catches it and displays it
                throw;
            }
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