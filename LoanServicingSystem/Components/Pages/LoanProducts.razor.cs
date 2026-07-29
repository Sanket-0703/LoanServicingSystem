using CoreData;
using CoreData.LoanOrigination;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanProducts : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        // =========================================
        // Page Data
        // =========================================

        public bool IsLoading { get; set; } = true;

        public bool IsSaving { get; set; } = false;

        public List<LoanProduct> AllProducts { get; set; } = new();

        public LoanProduct ActiveProduct { get; set; } = new();

        // =========================================
        // Modal State
        // =========================================

        public bool ShowModal { get; set; } = false;

        public bool IsUpdate { get; set; } = false;

        public string? ModalErrorMessage { get; set; }

        // =========================================
        // Dashboard Summary Metrics
        // =========================================

        protected int TotalProducts =>
            AllProducts.Count;

        protected decimal AverageInterest =>
            AllProducts.Any()
                ? Math.Round(AllProducts.Average(x => x.InterestRate), 2)
                : 0;

        protected int MaximumTenure =>
            AllProducts.Any()
                ? AllProducts.Max(x => x.TenureMonths)
                : 0;

        protected decimal AverageFee =>
            AllProducts.Any()
                ? Math.Round(AllProducts.Average(x => x.ProcessingFee ?? 0), 2)
                : 0;

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads all loan products when the page is initialized.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        // =========================================
        // Data Loading
        // =========================================

        /// <summary>
        /// Retrieves all configured loan products.
        /// </summary>
        private async Task LoadDataAsync()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    AllProducts = await LoanProduct.GetAllAsync(
                        DatabaseConnection);
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        // =========================================
        // Modal Actions
        // =========================================

        /// <summary>
        /// Opens the modal for creating a new loan product.
        /// </summary>
        protected void HandleCreateProduct()
        {
            IsUpdate = false;
            ModalErrorMessage = null;

            ActiveProduct = new LoanProduct
            {
                InterestRate = 10.0m,
                TenureMonths = 24,
                ProcessingFee = 1.0m
            };

            ShowModal = true;

            StateHasChanged();
        }

        /// <summary>
        /// Opens the modal for editing an existing loan product.
        /// </summary>
        protected void HandleConfigureProduct(Guid productId)
        {
            var product = AllProducts.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                IsUpdate = true;
                ModalErrorMessage = null;

                // Clone the entity to avoid modifying
                // the grid before saving.
                ActiveProduct = new LoanProduct
                {
                    Id = product.Id,
                    Name = product.Name,
                    InterestRate = product.InterestRate,
                    TenureMonths = product.TenureMonths,
                    ProcessingFee = product.ProcessingFee,
                    PenaltyRules = product.PenaltyRules
                };

                ShowModal = true;
            }
        }

        /// <summary>
        /// Closes the product dialog.
        /// </summary>
        protected void CloseModal()
        {
            ShowModal = false;
        }

        // =========================================
        // Data Operations
        // =========================================

        /// <summary>
        /// Creates a new loan product or updates
        /// an existing one.
        /// </summary>
        protected async Task SaveProductAsync()
        {
            ModalErrorMessage = null;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(ActiveProduct.Name) ||
                ActiveProduct.TenureMonths <= 0)
            {
                ModalErrorMessage =
                    "Please fill in all required fields with valid values.";

                return;
            }

            IsSaving = true;

            try
            {
                if (DatabaseConnection == null)
                    throw new Exception("Database connection missing.");

                ActiveProduct.UpdatedBy = "SystemAdmin";

                if (IsUpdate)
                {
                    await LoanProduct.UpdateAsync(
                        DatabaseConnection,
                        ActiveProduct);
                }
                else
                {
                    ActiveProduct.Id = Guid.NewGuid();

                    await LoanProduct.InsertAsync(
                        DatabaseConnection,
                        ActiveProduct);
                }

                ShowModal = false;

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ModalErrorMessage =
                    $"Error saving product: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        // =========================================
        // UI Helper Methods
        // =========================================

        /// <summary>
        /// Returns an icon based on the loan product type.
        /// </summary>
        protected string GetProductIcon(string name)
        {
            name = name.ToLower();

            if (name.Contains("home"))
                return "🏠";

            if (name.Contains("vehicle"))
                return "🚗";

            if (name.Contains("education"))
                return "🎓";

            if (name.Contains("personal"))
                return "💼";

            if (name.Contains("business"))
                return "🏢";

            return "💳";
        }
    }
}