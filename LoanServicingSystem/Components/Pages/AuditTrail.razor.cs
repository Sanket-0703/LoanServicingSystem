using CoreData;
using Microsoft.AspNetCore.Components;
using static CoreData.Identity.Users;

namespace LoanServicingSystem.Components.Pages
{
    public partial class AuditTrail : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;

        private List<SystemAuditLog> AllLogs { get; set; } = new();
        public List<SystemAuditLog> FilteredLogs { get; set; } = new();

        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; ApplyFilters(); }
        }

        private string _selectedSeverity = "All Severity Levels";
        public string SelectedSeverity
        {
            get => _selectedSeverity;
            set { _selectedSeverity = value; ApplyFilters(); }
        }

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    AllLogs = await SystemAuditLog.GetLogsAsync(DatabaseConnection);
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
            var query = AllLogs.AsEnumerable();

            if (SelectedSeverity != "All Severity Levels")
            {
                query = query.Where(l => l.Severity.Equals(SelectedSeverity, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var lowerSearch = SearchTerm.ToLowerInvariant();
                query = query.Where(l =>
                    l.ActorName.ToLowerInvariant().Contains(lowerSearch) ||
                    l.ActorEmail.ToLowerInvariant().Contains(lowerSearch) ||
                    l.EventAction.ToLowerInvariant().Contains(lowerSearch) ||
                    l.IpAddress.ToLowerInvariant().Contains(lowerSearch));
            }

            FilteredLogs = query.ToList();
        }

        protected string GetSeverityBadgeClass(string severity)
        {
            return severity.ToLowerInvariant() switch
            {
                "critical" => "bg-red-100 text-red-800",
                "warning" => "bg-amber-100 text-amber-800",
                _ => "bg-blue-100 text-blue-800"
            };
        }
    }
}