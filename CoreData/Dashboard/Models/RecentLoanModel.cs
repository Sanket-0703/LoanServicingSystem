namespace CoreData.Dashboard.Models;

public class RecentLoanModel
{
    public string LoanNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Principal { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }
}