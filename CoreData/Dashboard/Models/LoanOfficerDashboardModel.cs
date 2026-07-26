namespace CoreData.Dashboard.Models;

public class LoanOfficerDashboardModel
{
    // KPI Cards
    public int TotalApplications { get; set; }

    public int PendingApprovalCount { get; set; }

    public int ApprovedCount { get; set; }

    public int RejectedCount { get; set; }

    // Charts
    public List<CollectionTrendModel> WeeklyApplications { get; set; } = new();

    public List<LoanStatusChartModel> ApplicationStatus { get; set; } = new();

    // Tables
    public List<RecentLoanModel> MyApplications { get; set; } = new();

    public List<RecentLoanModel> PendingApprovals { get; set; } = new();
}