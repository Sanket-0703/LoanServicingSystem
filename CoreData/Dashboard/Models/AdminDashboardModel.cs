using CoreData.Dashboard.Models;

public class AdminDashboardModel
{
    // KPI Cards
    public int ActiveLoanCount { get; set; }

    public decimal TotalPortfolio { get; set; }

    public int CustomerCount { get; set; }

    public decimal CollectionsToday { get; set; }

    public List<CollectionTrendModel> CollectionTrend { get; set; } = new();

    // Portfolio
    public PortfolioSummaryModel Portfolio { get; set; } = new();

    // Tables
    public List<RecentLoanModel> RecentLoans { get; set; } = new();

    public List<RecentPaymentModel> RecentPayments { get; set; } = new();
    public List<LoanStatusChartModel> LoanStatus { get; set; } = new();


}