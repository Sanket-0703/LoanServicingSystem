using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class AuditTrailTable : ComponentBase
{
    [Parameter]
    public string Title { get; set; } = "Recent Audit Logs";

    [Parameter]
    public List<AuditTrailModel> Logs { get; set; } = new();
}