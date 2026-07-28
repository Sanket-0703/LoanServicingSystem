using Dapper;

namespace CoreData.Dashboard.Models;

public class AuditTrailModel
{
    public DateTime ChangedOn { get; set; }

    public string ChangedBy { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string ChangeType { get; set; } = string.Empty;

    public string RecordId { get; set; } = string.Empty;

    public string RecordNumber { get; set; } = string.Empty;

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

        // Repayment Schedules
        auditTrail.AddRange(await connection.QueryAsync<AuditTrailModel>(@"
        SELECT
            ChangedOn,
            ChangedBy,
            'Repayment Schedule' AS Module,
            ChangeType,
            CAST(Id AS NVARCHAR(100)) AS RecordId,
            CONCAT('EMI-', EmiNo) AS RecordNumber
        FROM RepaymentSchedulesAuditTrail"));

        return auditTrail
            .OrderByDescending(x => x.ChangedOn)
            .ToList();
    }
}