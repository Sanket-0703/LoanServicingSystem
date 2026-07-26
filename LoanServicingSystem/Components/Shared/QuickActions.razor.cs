using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class QuickActions
{
    [Parameter]
    public List<QuickActionModel> Actions { get; set; } = new();
}