using CoreData;
using CoreData.LoanOrigination;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class StatementGeneration : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoadingLoans { get; set; } = true;
        public bool IsGenerating { get; set; } = false;

        public List<Loan> AvailableLoans { get; set; } = new();
        public Guid SelectedLoanId { get; set; }
        public string StatementType { get; set; } = "Full Account Ledger Statement";
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-6);
        public DateTime EndDate { get; set; } = DateTime.Today;

        public StatementDetailsDto? StatementData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoadingLoans = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    AvailableLoans = await Loan.GetAllWithDetailsAsync(DatabaseConnection);
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

        protected async Task GeneratePreviewAsync()
        {
            if (SelectedLoanId == Guid.Empty || DatabaseConnection == null) return;

            IsGenerating = true;
            try
            {
                StatementData = await Statement.GetStatementDataAsync(DatabaseConnection, SelectedLoanId, StartDate, EndDate);
            }
            finally
            {
                IsGenerating = false;
                StateHasChanged();
            }
        }
    }
}