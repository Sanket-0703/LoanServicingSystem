using CoreData;
using CoreData.LoanOrigination;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanDirectory : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;
        public List<Loan> AllLoans { get; set; } = new();
        public IEnumerable<Loan> FilteredLoans { get; set; } = Array.Empty<Loan>();

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; ApplyFilters(); }
        }

        private string _selectedStatus = string.Empty;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; ApplyFilters(); }
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
                    AllLoans = await Loan.GetAllWithDetailsAsync(DatabaseConnection);
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
            var query = AllLoans.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(l =>
       l.LoanNumber.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
    || l.CustomerName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
    || l.ProductName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedStatus))
            {
                query = query.Where(l => l.Status.Equals(SelectedStatus, StringComparison.OrdinalIgnoreCase));
            }

            FilteredLoans = query.ToList();
        }

        protected string GetStatusBadgeClass(string status) => status switch
        {
            "Disbursed" => "bg-emerald-100 text-emerald-800",
            "Approved" => "bg-blue-100 text-blue-800",
            "Draft" => "bg-slate-100 text-slate-800",
            "Closed" => "bg-purple-100 text-purple-800",
            "Rejected" => "bg-rose-100 text-rose-800",
            _ => "bg-gray-100 text-gray-800"
        };

        protected int TotalLoans => AllLoans.Count;

        protected int DraftLoans =>
    AllLoans.Count(x => x.Status == "Draft");

        protected int ApprovedLoans =>
            AllLoans.Count(x => x.Status == "Approved");

        protected int DisbursedLoans =>
            AllLoans.Count(x => x.Status == "Disbursed");



    }
}