using CoreData;
using Microsoft.AspNetCore.Components;
using static CoreData.LoanOrigination.Loan;

namespace LoanServicingSystem.Components.Pages
{
    public partial class Dashboard : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;
        public DashboardMetrics Metrics { get; set; } = new();

        // UI Colors for the dynamic product distribution chart
        private readonly string[] ChartColors = { "bg-indigo-500", "bg-blue-400", "bg-emerald-400", "bg-amber-400", "bg-purple-500" };
        private readonly string[] SvgColors = { "text-indigo-500", "text-blue-400", "text-emerald-400", "text-amber-400", "text-purple-500" };

        protected override async Task OnInitializedAsync()
        {
            await LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    Metrics = await DashboardMetrics.GetLiveMetricsAsync(DatabaseConnection, DateTime.Today);
                    CalculateChartMetrics();
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private void CalculateChartMetrics()
        {
            // 1. Calculate Product Percentages
            var totalLoans = Metrics.ProductDistribution.Sum(p => p.LoanCount);
            if (totalLoans > 0)
            {
                foreach (var prod in Metrics.ProductDistribution)
                {
                    prod.Percentage = Math.Round((double)prod.LoanCount / totalLoans * 100, 1);
                }
            }

            // 2. Calculate Bar Chart Heights (Normalize to 0-100 scale)
            var maxCollection = Metrics.Last7DaysCollections.Any()
                ? Metrics.Last7DaysCollections.Max(c => c.TotalAmount)
                : 0;

            foreach (var day in Metrics.Last7DaysCollections)
            {
                if (maxCollection > 0)
                {
                    day.ChartHeight = Math.Max(5, (int)((day.TotalAmount / maxCollection) * 95));
                }
                else
                {
                    day.ChartHeight = 0;
                }
            }
        }

        protected string GetBgColor(int index) => ChartColors[index % ChartColors.Length];
        protected string GetSvgColor(int index) => SvgColors[index % SvgColors.Length];

        protected int GetDashOffset(int index)
        {
            if (index == 0) return 0;

            double offset = 0;
            for (int i = 0; i < index; i++)
            {
                offset += Metrics.ProductDistribution[i].Percentage;
            }
            return -(int)Math.Round(offset);
        }
    }
}