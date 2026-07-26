namespace CoreData.Dashboard.Models;

public class CollectionsDashboardModel
{
    // KPI Cards
    public int OverdueLoanCount { get; set; }

    public int DueTodayCount { get; set; }

    public decimal CollectedToday { get; set; }

    public decimal UnpaidPenaltyAmount { get; set; }

    // Charts
    public List<CollectionTrendModel> CollectionTrend { get; set; } = new();

    public List<LoanStatusChartModel> OverdueStatus { get; set; } = new();

    // Tables
    public List<RecentLoanModel> OverdueLoans { get; set; } = new();

    public List<RecentPaymentModel> RecentPayments { get; set; } = new();
}