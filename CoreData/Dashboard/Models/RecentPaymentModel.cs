namespace CoreData.Dashboard.Models;

public class RecentPaymentModel
{
    public string LoanNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMode { get; set; } = string.Empty;
}