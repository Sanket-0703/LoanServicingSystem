using CoreData.Dashboard.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages;

public partial class AuditorDashboard : ComponentBase
{
    [Inject]
    public IDashboardRepository DashboardRepository { get; set; } = default!;

    protected AuditorDashboardModel Dashboard { get; set; } = new();

    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Dashboard = await DashboardRepository.GetAuditorDashboardAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }
}