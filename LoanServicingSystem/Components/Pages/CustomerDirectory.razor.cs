using CoreData;
using CoreData.CustomerManagement;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages
{
    public partial class CustomerDirectory : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        // =========================================
        // Page Data
        // =========================================

        public bool IsLoading { get; set; } = true;
        public Guid UserId { get; private set; }
        public List<Customer> AllCustomers { get; set; } = new();

        public IEnumerable<Customer> FilteredCustomers { get; set; } = Array.Empty<Customer>();

        // =========================================
        // Filter Properties
        // =========================================

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                ApplyFilters();
            }
        }

        private string _selectedKycStatus = "All KYC Statuses";
        public string SelectedKycStatus
        {
            get => _selectedKycStatus;
            set
            {
                _selectedKycStatus = value;
                ApplyFilters();
            }
        }

        // =========================================
        // Dashboard Summary Metrics
        // =========================================

        protected int TotalCustomers =>
            AllCustomers.Count;

        protected int VerifiedCustomers =>
            AllCustomers.Count(c => c.KycStatus == "Verified");

        protected int PendingCustomers =>
            AllCustomers.Count(c => c.KycStatus == "Pending");

        protected int ActiveBorrowers =>
            AllCustomers.Count(c => c.ActiveLoansCount > 0);

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads customer data when the page is initialized.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        // =========================================
        // Data Loading Methods
        // =========================================

        /// <summary>
        /// Retrieves all customers and prepares the filtered view.
        /// </summary>
        private async Task LoadDataAsync()
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

                        AllCustomers = await Customer.GetAllCustomerUderLoanOfficer(DatabaseConnection, UserId);

                        ApplyFilters();
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
        // Filtering Methods
        // =========================================

        /// <summary>
        /// Applies search and KYC filters to the customer list.
        /// </summary>
        private void ApplyFilters()
        {
            var query = AllCustomers.AsEnumerable();

            // Search by customer name, email or ID
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null &&
                     c.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    c.Id.ToString().Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by KYC status
            if (SelectedKycStatus != "All KYC Statuses")
            {
                query = query.Where(c => c.KycStatus == SelectedKycStatus);
            }

            FilteredCustomers = query.ToList();
        }

        // =========================================
        // UI Helper Methods
        // =========================================

        /// <summary>
        /// Returns the badge styling for the customer's KYC status.
        /// </summary>
        protected string GetKycBadgeClass(string status) => status switch
        {
            "Verified" => "bg-emerald-100 text-emerald-800",
            "Pending" => "bg-amber-100 text-amber-800",
            "Rejected" => "bg-rose-100 text-rose-800",
            _ => "bg-slate-100 text-slate-800"
        };
    }
}