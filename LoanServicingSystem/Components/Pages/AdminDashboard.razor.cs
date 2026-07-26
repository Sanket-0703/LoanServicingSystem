using CoreData.Dashboard.Interfaces;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;
namespace LoanServicingSystem.Components.Pages;

public partial class AdminDashboard : ComponentBase
{
    [Inject]
    public IDashboardRepository DashboardRepository { get; set; } = default!;

    protected AdminDashboardModel Dashboard { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;

    protected List<QuickActionModel> QuickActionsList { get; set; } = new();
    protected async Task ReloadDashboard()
    {
        IsLoading = true;

        Dashboard = await DashboardRepository.GetAdminDashboardAsync();

        QuickActionsList =
[
    new()
    {
        Title = "Customers",
        Description = "Manage Customers",
        Url = "/customers",
        Icon = "👥",
        BackgroundColor = "bg-blue-100"
    },

    new()
    {
        Title = "Loans",
        Description = "Manage Loans",
        Url = "/loans",
        Icon = "💰",
        BackgroundColor = "bg-green-100"
    },

    new()
    {
        Title = "Statements",
        Description = "View Statements",
        Url = "/statements",
        Icon = "📄",
        BackgroundColor = "bg-purple-100"
    },

    new()
    {
        Title = "Reports",
        Description = "Generate Reports",
        Url = "/reports",
        Icon = "📊",
        BackgroundColor = "bg-amber-100"
    }
];

        IsLoading = false;

        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await ReloadDashboard();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            Dashboard = new AdminDashboardModel();
        }
        finally
        {
            IsLoading = false;
        }
    }
}