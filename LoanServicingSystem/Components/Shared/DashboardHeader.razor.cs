using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class DashboardHeader
{
    [Parameter]
    public EventCallback OnRefresh { get; set; }

    [Parameter]
    public string Title { get; set; } = "Loan Officer";

    protected string CurrentDate =>
        DateTime.Now.ToString("dddd, dd MMMM yyyy");

    protected async Task RefreshDashboard()
    {
        if (OnRefresh.HasDelegate)
            await OnRefresh.InvokeAsync();
    }
}