using CoreData;
using CoreData.LoanOrigination;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanProducts : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;
        public List<LoanProduct> AllProducts { get; set; } = new();

        public bool ShowModal { get; set; } = false;
        public bool IsUpdate { get; set; } = false;
        public bool IsSaving { get; set; } = false;
        public string? ModalErrorMessage { get; set; }

        public LoanProduct ActiveProduct { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    AllProducts = await LoanProduct.GetAllAsync(DatabaseConnection);
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected void HandleCreateProduct()
        {
            IsUpdate = false;
            ModalErrorMessage = null;
            ActiveProduct = new LoanProduct { InterestRate = 10.0m, TenureMonths = 24, ProcessingFee = 1.0m };
            ShowModal = true;
            StateHasChanged();
        }

        protected void HandleConfigureProduct(Guid productId)
        {
            var product = AllProducts.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                IsUpdate = true;
                ModalErrorMessage = null;
                // Clone the entity to prevent direct mutation of the UI grid before save
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

        protected void CloseModal()
        {
            ShowModal = false;
        }

        protected async Task SaveProductAsync()
        {
            ModalErrorMessage = null;

            // Manual Validation
            if (string.IsNullOrWhiteSpace(ActiveProduct.Name) || ActiveProduct.TenureMonths <= 0)
            {
                ModalErrorMessage = "Please fill in all required fields with valid values.";
                return;
            }

            IsSaving = true;
            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection missing.");

                ActiveProduct.UpdatedBy = "SystemAdmin";

                if (IsUpdate)
                {
                    await LoanProduct.UpdateAsync(DatabaseConnection, ActiveProduct);
                }
                else
                {
                    ActiveProduct.Id = Guid.NewGuid();
                    await LoanProduct.InsertAsync(DatabaseConnection, ActiveProduct);
                }

                ShowModal = false;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ModalErrorMessage = $"Error saving product: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        protected int TotalProducts => AllProducts.Count;

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