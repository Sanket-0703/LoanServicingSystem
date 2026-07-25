using CoreData;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoanServicingSystem.Components.Pages
{
    public partial class ReportsHub : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }
        [Inject] private IJSRuntime? JSRuntime { get; set; }

        public bool IsGenerating { get; set; } = false;

        public List<GeneratedReportLog> RecentReports { get; set; } = new();

        protected override void OnInitialized()
        {
            // Seed a dummy row for UI visual purposes
            RecentReports.Add(new GeneratedReportLog
            {
                FileName = $"Monthly_NPA_Summary_{DateTime.Now.ToString("MMM")}{DateTime.Now.Year}.csv",
                GeneratedBy = "System Schedule",
                Timestamp = DateTime.Now.AddHours(-12),
                Format = "CSV"
            });
        }

        protected async Task GenerateNpaReportAsync()
        {
            if (DatabaseConnection == null || JSRuntime == null) return;
            IsGenerating = true;

            try
            {
                var data = await ReportEngine.GetNpaReportAsync(DatabaseConnection);

                var csv = new StringBuilder();
                csv.AppendLine("LoanNumber,BorrowerName,OriginalPrincipal,DaysPastDue,TotalOverdue");

                foreach (var row in data)
                {
                    csv.AppendLine($"{row.LoanNumber},\"{row.BorrowerName}\",{row.OriginalPrincipal:F2},{row.DaysPastDue},{row.TotalOverdue:F2}");
                }

                var fileName = $"NPA_Report_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, csv.ToString());
                LogReportGeneration(fileName);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        protected async Task GenerateInterestReportAsync()
        {
            if (DatabaseConnection == null || JSRuntime == null) return;
            IsGenerating = true;

            try
            {
                var data = await ReportEngine.GetInterestEarnedReportAsync(DatabaseConnection);

                var csv = new StringBuilder();
                csv.AppendLine("ProductName,ActiveAccounts,TotalInterestAccrued");

                foreach (var row in data)
                {
                    csv.AppendLine($"\"{row.ProductName}\",{row.ActiveAccounts},{row.TotalInterestAccrued:F2}");
                }

                var fileName = $"Interest_Accrual_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, csv.ToString());
                LogReportGeneration(fileName);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        protected async Task GenerateCollectionsReportAsync()
        {
            if (DatabaseConnection == null || JSRuntime == null) return;
            IsGenerating = true;

            try
            {
                var data = await ReportEngine.GetDailyCollectionsReconAsync(DatabaseConnection, DateTime.Today);

                var csv = new StringBuilder();
                csv.AppendLine("TransactionDate,LoanNumber,AmountCollected,PaymentMethod,BankReference");

                foreach (var row in data)
                {
                    csv.AppendLine($"{row.TransactionDate:yyyy-MM-dd HH:mm},{row.LoanNumber},{row.AmountCollected:F2},\"{row.PaymentMethod}\",\"{row.BankReference}\"");
                }

                var fileName = $"Daily_Collections_Recon_{DateTime.Now:yyyyMMdd}.csv";
                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, csv.ToString());
                LogReportGeneration(fileName);
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private void LogReportGeneration(string fileName)
        {
            RecentReports.Insert(0, new GeneratedReportLog
            {
                FileName = fileName,
                GeneratedBy = "Current User",
                Timestamp = DateTime.Now,
                Format = "CSV"
            });

            if (RecentReports.Count > 10) RecentReports.RemoveAt(RecentReports.Count - 1);
        }

        public class GeneratedReportLog
        {
            public string FileName { get; set; } = string.Empty;
            public string GeneratedBy { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string Format { get; set; } = string.Empty;
        }
    }
}