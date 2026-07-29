using CoreData;
using CoreData.CustomerManagement;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages
{
    public partial class CustomerProfile : ComponentBase
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
        // Route Parameters
        // =========================================

        /// <summary>
        /// Customer identifier used when editing an existing record.
        /// </summary>
        [Parameter]
        public Guid? Id { get; set; }

        // =========================================
        // Page Data
        // =========================================

        public bool IsLoading { get; set; } = true;
        public Guid UserId { get; private set; }
        public bool IsSaving { get; set; } = false;

        public string? ErrorMessage { get; set; }

        public Customer ActiveCustomer { get; set; } = new();

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads the customer profile when editing an existing customer.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            try
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

                if (authState.User.Identity?.IsAuthenticated == true)
                {
                    UserId = Users.GetCurrentUserId(authState.User);
                }
                if (Id.HasValue && DatabaseConnection != null)
                {
                    var customer = await Customer.GetByIdAsync(
                        DatabaseConnection,
                        Id.Value);
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

        // =========================================
        // Data Operations
        // =========================================

        /// <summary>
        /// Creates a new customer or updates an existing customer profile.
        /// </summary>
        protected async Task SaveCustomerAsync()
        {
            ErrorMessage = null;

            // Validate mandatory fields
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
                if (DatabaseConnection == null)
                    throw new Exception("Database connection missing.");

                ActiveCustomer.CreatedBy = UserId;

                if (Id.HasValue)
                {
                    await Customer.UpdateAsync(
                        DatabaseConnection,
                        ActiveCustomer);
                }
                else
                {
                    ActiveCustomer.Id = Guid.NewGuid();

                    await Customer.InsertAsync(
                        DatabaseConnection,
                        ActiveCustomer);
                }

                // Return to the customer directory
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

        // =========================================
        // Navigation
        // =========================================

        /// <summary>
        /// Returns to the customer directory.
        /// </summary>
        protected void GoBack()
        {
            NavigationManager?.NavigateTo("/customers");
        }



    }
}