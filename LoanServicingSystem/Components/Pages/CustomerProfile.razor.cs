using CoreData;
using CoreData.CustomerManagement;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class CustomerProfile : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }
        [Inject] private NavigationManager? NavigationManager { get; set; }

        // Captures the optional {Id} from the URL route
        [Parameter] public Guid? Id { get; set; }

        public bool IsLoading { get; set; } = true;
        public bool IsSaving { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public Customer ActiveCustomer { get; set; } = new Customer();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                if (Id.HasValue && DatabaseConnection != null)
                {
                    var customer = await Customer.GetByIdAsync(DatabaseConnection, Id.Value);
                    if (customer != null)
                    {
                        ActiveCustomer = customer;
                    }
                    else
                    {
                        ErrorMessage = "Customer record not found.";
                    }
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected async Task SaveCustomerAsync()
        {
            ErrorMessage = null;

            // Manual validation for NOT NULL database constraints
            if (string.IsNullOrWhiteSpace(ActiveCustomer.Name) ||
                string.IsNullOrWhiteSpace(ActiveCustomer.Phone) ||
                string.IsNullOrWhiteSpace(ActiveCustomer.PAN))
            {
                ErrorMessage = "Name, Phone, and PAN are required fields.";
                return;
            }

            IsSaving = true;
            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection missing.");

                ActiveCustomer.UpdatedBy = "SystemAdmin";

                if (Id.HasValue)
                {
                    await Customer.UpdateAsync(DatabaseConnection, ActiveCustomer);
                }
                else
                {
                    ActiveCustomer.Id = Guid.NewGuid();
                    await Customer.InsertAsync(DatabaseConnection, ActiveCustomer);
                }

                // Redirect back to the directory on success
                NavigationManager?.NavigateTo("/customers");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Database constraint error: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        protected void GoBack()
        {
            NavigationManager?.NavigateTo("/customers");
        }
    }
}