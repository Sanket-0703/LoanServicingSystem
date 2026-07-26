namespace CoreData.Dashboard.Models;

public class AuditTrailModel
{
    public DateTime ChangedOn { get; set; }

    public string ChangedBy { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string ChangeType { get; set; } = string.Empty;

    public string RecordId { get; set; } = string.Empty;
}