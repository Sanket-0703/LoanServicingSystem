namespace CoreData.Dashboard.Models;

public class PortfolioSummaryModel
{
    public int DraftLoanCount { get; set; }

    public int ActiveLoanCount { get; set; }

    public int ClosedLoanCount { get; set; }

    public int OverdueLoanCount { get; set; }
}