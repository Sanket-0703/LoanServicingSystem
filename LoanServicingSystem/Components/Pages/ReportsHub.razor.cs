using System.Text;
using CoreData;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LoanServicingSystem.Components.Pages
{
    public partial class ReportsHub : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private IJSRuntime? JSRuntime { get; set; }

        // =========================================
        // Page State
        // =========================================

        public bool IsGenerating { get; set; } = false;

        public List<GeneratedReportLog> RecentReports { get; set; } = new();

        // =========================================
        // Dashboard Statistics
        // =========================================

        public int TotalReports =>
            RecentReports.Count;

        public int CsvReports =>
            RecentReports.Count(x => x.Format == "CSV");

        public int TodayReports =>
            RecentReports.Count(x => x.Timestamp.Date == DateTime.Today);

        public int ScheduledReports =>
            RecentReports.Count(x => x.GeneratedBy == "System Schedule");

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Seeds a sample report entry for UI display.
        /// </summary>
        protected override void OnInitialized()
        {
            RecentReports.Add(new GeneratedReportLog
            {
                FileName = $"Monthly_NPA_Summary_{DateTime.Now:MMM}{DateTime.Now.Year}.csv",
                GeneratedBy = "System Schedule",
                Timestamp = DateTime.Now.AddHours(-12),
                Format = "CSV"
            });
        }

        // =========================================
        // Report Generation
        // =========================================

        /// <summary>
        /// Generates the Non-Performing Asset (NPA) report.
        /// </summary>
        protected async Task GenerateNpaReportAsync()
        {
            if (DatabaseConnection == null || JSRuntime == null)
                return;

            IsGenerating = true;

            try
            {
                var data = await ReportEngine.GetNpaReportAsync(DatabaseConnection);

                var csv = new StringBuilder();

                csv.AppendLine("LoanNumber,BorrowerName,OriginalPrincipal,DaysPastDue,TotalOverdue");

                foreach (var row in data)
                {
                    csv.AppendLine(
                        $"{row.LoanNumber},\"{row.BorrowerName}\",{row.OriginalPrincipal:F2},{row.DaysPastDue},{row.TotalOverdue:F2}");
                }

                var fileName = $"NPA_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                await JSRuntime.InvokeVoidAsync(
                    "downloadFile",
                    fileName,
                    csv.ToString());

                LogReportGeneration(fileName);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        /// <summary>
        /// Generates the interest accrual report.
        /// </summary>
        protected async Task GenerateInterestReportAsync()
        {
            if (DatabaseConnection == null || JSRuntime == null)
                return;

            IsGenerating = true;

            try
            {
                var data = await ReportEngine.GetInterestEarnedReportAsync(DatabaseConnection);

                var csv = new StringBuilder();

                csv.AppendLine("ProductName,ActiveAccounts,TotalInterestAccrued");

                foreach (var row in data)
                {
                    csv.AppendLine(
                        $"\"{row.ProductName}\",{row.ActiveAccounts},{row.TotalInterestAccrued:F2}");
                }

                var fileName = $"Interest_Accrual_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                await JSRuntime.InvokeVoidAsync(
                    "downloadFile",
                    fileName,
                    csv.ToString());

                LogReportGeneration(fileName);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        /// <summary>
        /// Generates the daily collections reconciliation report.
        /// </summary>
        protected async Task GenerateCollectionsReportAsync()
        {
            if (DatabaseConnection == null || JSRuntime == null)
                return;

            IsGenerating = true;

            try
            {
                var data = await ReportEngine.GetDailyCollectionsReconAsync(
                    DatabaseConnection,
                    DateTime.Today);

                var csv = new StringBuilder();

                csv.AppendLine("TransactionDate,LoanNumber,AmountCollected,PaymentMethod,BankReference");

                foreach (var row in data)
                {
                    csv.AppendLine(
                        $"{row.TransactionDate:yyyy-MM-dd HH:mm},{row.LoanNumber},{row.AmountCollected:F2},\"{row.PaymentMethod}\",\"{row.BankReference}\"");
                }

                var fileName = $"Daily_Collections_Recon_{DateTime.Now:yyyyMMdd}.csv";

                await JSRuntime.InvokeVoidAsync(
                    "downloadFile",
                    fileName,
                    csv.ToString());

                LogReportGeneration(fileName);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        // =========================================
        // Helper Methods
        // =========================================

        /// <summary>
        /// Adds the generated report to the recent reports list.
        /// </summary>
        private void LogReportGeneration(string fileName)
        {
            RecentReports.Insert(0, new GeneratedReportLog
            {
                FileName = fileName,
                GeneratedBy = "Current User",
                Timestamp = DateTime.Now,
                Format = "CSV"
            });

            // Keep only the latest 10 reports
            if (RecentReports.Count > 10)
            {
                RecentReports.RemoveAt(RecentReports.Count - 1);
            }
        }

        // =========================================
        // View Models
        // =========================================

        /// <summary>
        /// Represents a generated report displayed in the UI.
        /// </summary>
        public class GeneratedReportLog
        {
            public string FileName { get; set; } = string.Empty;

            public string GeneratedBy { get; set; } = string.Empty;

            public DateTime Timestamp { get; set; }

            public string Format { get; set; } = string.Empty;
        }
    }
}