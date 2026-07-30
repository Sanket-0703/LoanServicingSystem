using CoreData;
using CoreData.Export;
using CoreData.LoanOrigination;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LoanServicingSystem.Components.Pages
{
    public partial class StatementGeneration : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        // =========================================
        // Page State
        // =========================================

        public bool IsLoadingLoans { get; set; } = true;

        public bool IsGenerating { get; set; } = false;

        // =========================================
        // Statement Filters
        // =========================================

        public List<Loan> AvailableLoans { get; set; } = new();

        public Guid SelectedLoanId { get; set; }

        public Loan? SelectedLoan { get; set; }

        public int TotalPayments { get; set; }



        // =========================================
        // Generated Statement
        // =========================================



        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads available loans and generates
        /// the initial statement preview.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            IsLoadingLoans = true;

            try
            {
                if (DatabaseConnection == null)
                    return;

                AvailableLoans =
                    await Loan.GetAllWithDetailsAsync(DatabaseConnection);

                if (AvailableLoans.Any())
                {
                    SelectedLoanId = AvailableLoans.First().Id;

                    await LoadLoanAsync();
                }
            }
            finally
            {
                IsLoadingLoans = false;
            }
        }

        private async Task LoadLoanAsync()
        {
            if (DatabaseConnection == null)
                return;

            SelectedLoan =
                await Loan.GetLoanWorkspaceAsync(
                    DatabaseConnection,
                    SelectedLoanId);

            var payments =
                await Payment.GetByLoanIdAsync(
                    DatabaseConnection,
                    SelectedLoanId);

            TotalPayments = payments.Count;

            StateHasChanged();
        }

        private async Task OnLoanChanged(ChangeEventArgs e)
        {
            SelectedLoanId = Guid.Parse(e.Value!.ToString()!);

            await LoadLoanAsync();
        }

        // =========================================
        // Statement Generation
        // =========================================

        private async Task ExportStatement()
        {
            if (DatabaseConnection == null)
                return;

            IsGenerating = true;

            try
            {
                var loan = SelectedLoan;

                if (loan == null)
                    return;

                if (loan == null)
                    return;

                var payments =
                    await Payment.GetByLoanIdAsync(
                        DatabaseConnection,
                        SelectedLoanId);

                var schedules =
                    await RepaymentSchedule.GetByLoanIdAsync(
                        DatabaseConnection,
                        SelectedLoanId);

                var workbook =
                    LoanStatementExporter.GenerateWorkbook(
                        loan,
                        payments,
                        schedules);

                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                await JS.InvokeVoidAsync(
                    "downloadFile",
                    $"LoanStatement_{loan.LoanNumber}.xlsx",
                    Convert.ToBase64String(stream.ToArray()));
            }
            finally
            {
                IsGenerating = false;
            }
        }

    }
}