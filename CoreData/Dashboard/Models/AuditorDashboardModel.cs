using CoreData.Dashboard.Models;

public class AuditorDashboardModel
{
    // KPI Cards
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public decimal TotalCollections { get; set; }
    public int AuditRecordCount { get; set; }

    // Charts
    public List<LoanStatusChartModel> LoanStatus { get; set; } = new();

    public List<CollectionTrendModel> CollectionTrend { get; set; } = new();

    // Tables
    public List<AuditTrailModel> RecentAuditLogs { get; set; } = new();

    public List<RecentLoanModel> RecentLoans { get; set; } = new();
}