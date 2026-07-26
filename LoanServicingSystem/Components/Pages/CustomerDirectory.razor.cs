using CoreData;
using CoreData.CustomerManagement;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class CustomerDirectory : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;
        public List<Customer> AllCustomers { get; set; } = new();
        public IEnumerable<Customer> FilteredCustomers { get; set; } = Array.Empty<Customer>();

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; ApplyFilters(); }
        }

        private string _selectedKycStatus = "All KYC Statuses";
        public string SelectedKycStatus
        {
            get => _selectedKycStatus;
            set { _selectedKycStatus = value; ApplyFilters(); }
        }

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
                    AllCustomers = await Customer.GetAllWithActiveLoanCountAsync(DatabaseConnection);
                    ApplyFilters();
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private void ApplyFilters()
        {
            var query = AllCustomers.AsEnumerable();

            // Search by Name, Email, or the GUID
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null && c.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    c.Id.ToString().Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by computed KYC Status
            if (SelectedKycStatus != "All KYC Statuses")
            {
                query = query.Where(c => c.KycStatus == SelectedKycStatus);
            }

            FilteredCustomers = query.ToList();
        }

        protected string GetKycBadgeClass(string status) => status switch
        {
            "Verified" => "bg-emerald-100 text-emerald-800",
            "Pending" => "bg-amber-100 text-amber-800",
            "Rejected" => "bg-rose-100 text-rose-800",
            _ => "bg-slate-100 text-slate-800"
        };

        protected int TotalCustomers => AllCustomers.Count;

        protected int VerifiedCustomers =>
            AllCustomers.Count(c => c.KycStatus == "Verified");

        protected int PendingCustomers =>
            AllCustomers.Count(c => c.KycStatus == "Pending");

        protected int ActiveBorrowers =>
            AllCustomers.Count(c => c.ActiveLoansCount > 0);
    }
}