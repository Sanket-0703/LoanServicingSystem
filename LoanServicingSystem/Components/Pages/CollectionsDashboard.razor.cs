using CoreData.Dashboard.Interfaces;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Dashboards;

public partial class CollectionsDashboard : ComponentBase
{
    [Inject]
    public IDashboardRepository DashboardRepository { get; set; } = default!;

    protected CollectionsDashboardModel Dashboard { get; private set; } = new();

    protected bool IsLoading { get; private set; } = true;

    protected List<QuickActionModel> QuickActionsList { get; set; } = new()
    {
        new()
        {
            Title = "Record Payment",
            Icon = "💳",
            Url = "/payments"
        },

        new()
        {
            Title = "Apply Penalty",
            Icon = "⚠️",
            Url = "/penalties"
        },

        new()
        {
            Title = "Overdue Loans",
            Icon = "📋",
            Url = "/collections"
        },

        new()
        {
            Title = "Customer Details",
            Icon = "👤",
            Url = "/customers"
        }
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Dashboard = await DashboardRepository.GetCollectionsDashboardAsync();
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