using CoreData.Dashboard.Interfaces;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages;

public partial class AdminDashboard : ComponentBase
{
    // =========================================
    // Dependency Injection
    // =========================================

    [Inject]
    public IDashboardRepository DashboardRepository { get; set; } = default!;

    // =========================================
    // Dashboard Data
    // =========================================

    protected AdminDashboardModel Dashboard { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;

    protected List<QuickActionModel> QuickActionsList { get; set; } = new();

    // =========================================
    // Lifecycle Methods
    // =========================================

    /// <summary>
    /// Loads the dashboard when the page is opened.
    /// </summary>
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

    // =========================================
    // Dashboard Methods
    // =========================================

    /// <summary>
    /// Refreshes dashboard metrics and quick action cards.
    /// Invoked during initial page load and manual refresh.
    /// </summary>
    protected async Task ReloadDashboard()
    {
        IsLoading = true;

        // Load latest dashboard data
        Dashboard = await DashboardRepository.GetAdminDashboardAsync();

        // Configure dashboard quick action shortcuts
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
}