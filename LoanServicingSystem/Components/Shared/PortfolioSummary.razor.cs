using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class PortfolioSummary
{
    [Parameter]
    public PortfolioSummaryModel Model { get; set; } = new();
}