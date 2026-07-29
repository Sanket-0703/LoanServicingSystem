using CoreData;
using CoreData.LoanOrigination;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class StatementGeneration : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

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

        public string StatementType { get; set; } = "Full Account Ledger Statement";

        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-6);

        public DateTime EndDate { get; set; } = DateTime.Today;

        // =========================================
        // Generated Statement
        // =========================================

        public StatementDetailsDto? StatementData { get; set; }

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
                if (DatabaseConnection != null)
                {
                    // Load all available loans
                    AvailableLoans = await Loan.GetAllWithDetailsAsync(DatabaseConnection);

                    // Generate preview for the first loan
                    if (AvailableLoans.Any())
                    {
                        SelectedLoanId = AvailableLoans.First().Id;

                        await GeneratePreviewAsync();
                    }
                }
            }
            finally
            {
                IsLoadingLoans = false;

                StateHasChanged();
            }
        }

        // =========================================
        // Statement Generation
        // =========================================

        /// <summary>
        /// Generates a statement preview for the
        /// selected loan and date range.
        /// </summary>
        protected async Task GeneratePreviewAsync()
        {
            if (SelectedLoanId == Guid.Empty || DatabaseConnection == null)
                return;

            IsGenerating = true;

            try
            {
                StatementData = await Statement.GetStatementDataAsync(
                    DatabaseConnection,
                    SelectedLoanId,
                    StartDate,
                    EndDate);
            }
            finally
            {
                IsGenerating = false;

                StateHasChanged();
            }
        }
    }
}