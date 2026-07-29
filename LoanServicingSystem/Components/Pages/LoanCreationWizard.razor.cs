using CoreData;
using CoreData.CustomerManagement;
using CoreData.Identity;
using CoreData.LoanOrigination;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanCreationWizard : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        // =========================================
        // Page Data
        // =========================================

        public bool IsLoading { get; set; } = true;
        public Guid UserId { get; private set; }
        public bool IsSaving { get; set; } = false;

        public string? ErrorMessage { get; set; }

        public Loan NewLoan { get; set; } = new()
        {
            Status = "Draft",
            StartDate = DateTime.Today,
            RepaymentFrequency = "Monthly"
        };

        public List<Customer> Customers { get; set; } = new();

        public List<LoanProduct> LoanProducts { get; set; } = new();

        // =========================================
        // Wizard State
        // =========================================

        protected int CurrentStep { get; set; } = 1;

        protected Customer? SelectedCustomer =>
            Customers.FirstOrDefault(x => x.Id == NewLoan.CustomerId);

        protected LoanProduct? SelectedProduct =>
            LoanProducts.FirstOrDefault(x => x.Id == NewLoan.ProductId);

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads customers and loan products required for loan origination.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

                    if (authState.User.Identity?.IsAuthenticated == true)
                    {
                        UserId = Users.GetCurrentUserId(authState.User);

                        Customers = await Customer.GetAllCustomerUderLoanOfficer(DatabaseConnection, UserId);

                        LoanProducts = await LoanProduct.GetAllAsync(DatabaseConnection);
                    }

                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        // =========================================
        // Wizard Navigation
        // =========================================

        /// <summary>
        /// Moves the wizard to the next step after validation.
        /// </summary>
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

        /// <summary>
        /// Returns the wizard to the previous step.
        /// </summary>
        protected void PreviousStep()
        {
            ErrorMessage = null;

            if (CurrentStep > 1)
                CurrentStep--;
        }

        // =========================================
        // Form Actions
        // =========================================

        /// <summary>
        /// Updates loan details when a loan product is selected.
        /// </summary>
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

        /// <summary>
        /// Creates the loan and generates its repayment schedule.
        /// </summary>
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
                if (DatabaseConnection == null)
                    throw new Exception("Database connection missing.");

                NewLoan.Id = Guid.NewGuid();

                NewLoan.LoanNumber =
                    $"LN-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

                NewLoan.EndDate =
                    NewLoan.StartDate?.AddMonths(NewLoan.Tenure);

                NewLoan.UpdatedBy = "SystemAdmin";

                await Loan.OriginateLoanWithScheduleAsync(
                    DatabaseConnection,
                    NewLoan);

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