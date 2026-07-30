using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.Dashboard.Models;

public class AuditTrailModel
{
    public DateTime ChangedOn { get; set; }

    public string ChangedBy { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string ChangeType { get; set; } = string.Empty;

    public string RecordId { get; set; } = string.Empty;

    public string RecordNumber { get; set; } = string.Empty;

    [Computed]
    public string LoanNumber { get; set; } = string.Empty;

    [Computed]
    public string CustomerName { get; set; } = string.Empty;

    public static async Task<List<AuditTrailModel>> GetByLoanIdAsync(IDatabaseConnection databaseConnection, Guid loanId)
    {
        try
        {
            using var connection = databaseConnection.GetConnection();

            var auditTrail = new List<AuditTrailModel>();

            auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Loans' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            LoanNumber AS RecordNumber
        FROM LoansAuditTrail
        WHERE Id = @LoanId",
                new
                {
                    LoanId = loanId
                }));

            return auditTrail
                .OrderByDescending(x => x.ChangedOn)
                .ToList();
        }
        catch (Exception ex)
        {
            // Log the exact SQL error to the server console
            Console.WriteLine($"Error fetching audit trail for Loan {loanId}: {ex.Message}");

            // Rethrow the exception so LoadDataAsync catches it and displays it
            throw;
        }
    }
    public static async Task<List<AuditTrailModel>> GetAuditTrailAsync(
     IDatabaseConnection databaseConnection)
    {
        using var connection = databaseConnection.GetConnection();

        var auditTrail = new List<AuditTrailModel>();

        // Loans
        auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Loans' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            LoanNumber AS RecordNumber
        FROM LoansAuditTrail"));

        // Customers
        auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Customers' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            Name AS RecordNumber
        FROM CustomersAuditTrail"));

        // Payments
        auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Payments' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            ReferenceNumber AS RecordNumber
        FROM PaymentsAuditTrail"));

        // Loan Products
        auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Loan Products' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            Name AS RecordNumber
        FROM LoanProductsAuditTrail"));

        // Disbursements
        auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Disbursements' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            ReferenceNumber AS RecordNumber
        FROM DisbursementsAuditTrail"));



        return auditTrail
            .OrderByDescending(x => x.ChangedOn)
            .ToList();
    }
}