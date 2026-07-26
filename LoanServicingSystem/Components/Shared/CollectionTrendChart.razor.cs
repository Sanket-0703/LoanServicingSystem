using ApexCharts;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class CollectionTrendChart : ComponentBase
{
    [Parameter]
    public List<CollectionTrendModel> Data { get; set; } = new();

    protected ApexChartOptions<CollectionTrendModel> Options { get; set; } = new();

    protected override void OnInitialized()
    {
        Options = new ApexChartOptions<CollectionTrendModel>
        {
            Chart = new Chart
            {
                Toolbar = new Toolbar
                {
                    Show = false
                }
            },

            Stroke = new Stroke
            {
                Curve = Curve.Smooth,
                Width = 3
            },

            DataLabels = new DataLabels
            {
                Enabled = false
            }
        };
    }
}