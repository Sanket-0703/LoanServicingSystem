using CoreData.Dashboard.Interfaces;
using CoreData.Dashboard.Models;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
namespace LoanServicingSystem.Components.Pages;

public partial class LoanOfficerDashboard : ComponentBase
{
    // =========================================
    // Dependency Injection
    // =========================================

    [Inject]
    public IDashboardRepository DashboardRepository { get; set; } = default!;
    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    // =========================================
    // Page Data
    // =========================================

    protected LoanOfficerDashboardModel Dashboard { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;
    public Guid UserId { get; set; }

    // =========================================
    // Quick Actions
    // =========================================

    protected List<QuickActionModel> QuickActionsList { get; set; } =
    [
        new()
        {
            Title = "New Customer",
            Icon = "👤",
            Url = "/customers"
        },

        new()
        {
            Title = "New Loan",
            Icon = "💰",
            Url = "/loans"
        },

        new()
        {
            Title = "Upload Documents",
            Icon = "📄",
            Url = "/documents"
        },

        new()
        {
            Title = "Search Customer",
            Icon = "🔍",
            Url = "/customers"
        }
    ];

    // =========================================
    // Lifecycle Methods
    // =========================================

    /// <summary>
    /// Loads the dashboard when the page is initialized.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                UserId = Users.GetCurrentUserId(authState.User);


            }
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

    // =========================================
    // Data Loading
    // =========================================

    /// <summary>
    /// Reloads the latest dashboard data.
    /// </summary>
    protected async Task ReloadDashboard()
    {
        IsLoading = true;

        Dashboard = await DashboardRepository.GetLoanOfficerDashboardAsync(UserId);

        IsLoading = false;

        StateHasChanged();
    }
}