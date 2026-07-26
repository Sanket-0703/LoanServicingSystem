using CoreData.Dashboard.Interfaces;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages;

public partial class LoanOfficerDashboard : ComponentBase
{
    [Inject]
    public IDashboardRepository DashboardRepository { get; set; } = default!;

    protected LoanOfficerDashboardModel Dashboard { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;

    protected async Task ReloadDashboard()
    {
        IsLoading = true;

        Dashboard = await DashboardRepository.GetLoanOfficerDashboardAsync();

        IsLoading = false;

        StateHasChanged();
    }

    protected List<QuickActionModel> QuickActionsList { get; set; } = new()
    {
        new()
        {
            Title="New Customer",
            Icon="👤",
            Url="/customers"
        },

        new()
        {
            Title="New Loan",
            Icon="💰",
            Url="/loans"
        },

        new()
        {
            Title="Upload Documents",
            Icon="📄",
            Url="/documents"
        },

        new()
        {
            Title="Search Customer",
            Icon="🔍",
            Url="/customers"
        }
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await ReloadDashboard();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }
}