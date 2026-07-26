using CoreData;
using CoreData.CustomerManagement;
using CoreData.LoanOrigination;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanCreationWizard : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }
        [Inject] private NavigationManager? NavigationManager { get; set; }

        public bool IsLoading { get; set; } = true;
        public bool IsSaving { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public List<Customer> Customers { get; set; } = new();
        public List<LoanProduct> LoanProducts { get; set; } = new();



        protected Customer? SelectedCustomer =>
    Customers.FirstOrDefault(x => x.Id == NewLoan.CustomerId);

        protected LoanProduct? SelectedProduct =>
            LoanProducts.FirstOrDefault(x => x.Id == NewLoan.ProductId);

        protected int CurrentStep { get; set; } = 1;

        protected void NextStep()
        {
            if (CurrentStep == 1)
            {
                if (NewLoan.CustomerId == Guid.Empty ||
                    NewLoan.ProductId == Guid.Empty)
                {
                    ErrorMessage = "Please select customer and product.";
                    return;
                }
            }

            if (CurrentStep == 2)
            {
                if (NewLoan.Principal <= 0 ||
                    NewLoan.Tenure <= 0)
                {
                    ErrorMessage = "Enter valid loan amount and tenure.";
                    return;
                }
            }

            ErrorMessage = null;

            if (CurrentStep < 3)
                CurrentStep++;
        }

        protected void PreviousStep()
        {
            ErrorMessage = null;

            if (CurrentStep > 1)
                CurrentStep--;
        }

        public Loan NewLoan { get; set; } = new Loan
        {
            Status = "Draft",
            StartDate = DateTime.Today,
            RepaymentFrequency = "Monthly"
        };

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    Customers = await Customer.GetAllWithActiveLoanCountAsync(DatabaseConnection);
                    LoanProducts = await LoanProduct.GetAllAsync(DatabaseConnection);
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected void OnProductChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out Guid productId))
            {
                NewLoan.ProductId = productId;
                var selectedProduct = LoanProducts.Find(p => p.Id == productId);
                if (selectedProduct != null)
                {
                    NewLoan.InterestRate = selectedProduct.InterestRate;
                    NewLoan.Tenure = selectedProduct.TenureMonths;
                }
            }
        }

        protected async Task SubmitLoanAsync()
        {
            ErrorMessage = null;

            if (NewLoan.CustomerId == Guid.Empty ||
                NewLoan.ProductId == Guid.Empty ||
                NewLoan.Principal <= 0 ||
                NewLoan.Tenure <= 0)
            {
                ErrorMessage = "Please select a valid customer, product template, and enter positive principal and tenure values.";
                return;
            }

            IsSaving = true;
            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection missing.");

                NewLoan.Id = Guid.NewGuid();
                NewLoan.LoanNumber = $"LN-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
                NewLoan.EndDate = NewLoan.StartDate?.AddMonths(NewLoan.Tenure);
                NewLoan.UpdatedBy = "SystemAdmin";

                await Loan.OriginateLoanWithScheduleAsync(DatabaseConnection, NewLoan);

                NavigationManager?.NavigateTo("/loans");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error originating loan: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}