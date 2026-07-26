using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class RecentPaymentsTable
{
    [Parameter]
    public List<RecentPaymentModel> Payments { get; set; } = new();
    [Parameter]
    public string Title { get; set; } = "Recent Payments";
}