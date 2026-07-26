using ApexCharts;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class LoanStatusChart : ComponentBase
{
    [Parameter]
    public List<LoanStatusChartModel> Data { get; set; } = new();

    protected ApexChartOptions<LoanStatusChartModel> Options { get; set; } = new();

    protected override void OnInitialized()
    {
        Options = new ApexChartOptions<LoanStatusChartModel>
        {
            Chart = new Chart
            {
                Toolbar = new Toolbar
                {
                    Show = false
                }
            },

            PlotOptions = new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    Donut = new PlotOptionsDonut
                    {
                        Size = "70%"
                    }
                }
            },

            Legend = new Legend
            {
                Position = LegendPosition.Bottom
            },

            DataLabels = new DataLabels
            {
                Enabled = true
            }
        };
    }
}