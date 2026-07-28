using CoreData;
using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class AuditTrail : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;

        private List<AuditTrailModel> AllLogs { get; set; } = new();

        public List<AuditTrailModel> FilteredLogs { get; set; } = new();

        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                ApplyFilters();
            }
        }

        private string _selectedModule = "All Modules";
        public string SelectedModule
        {
            get => _selectedModule;
            set
            {
                _selectedModule = value;
                ApplyFilters();
            }
        }

        private string _selectedChangeType = "All Changes";
        public string SelectedChangeType
        {
            get => _selectedChangeType;
            set
            {
                _selectedChangeType = value;
                ApplyFilters();
            }
        }

        #region KPI Cards

        protected int TotalChanges =>
            AllLogs.Count;

        protected int TodayChanges =>
            AllLogs.Count(x => x.ChangedOn.Date == DateTime.Today);

        protected int InsertCount =>
            AllLogs.Count(x =>
                x.ChangeType.Equals("Insert", StringComparison.OrdinalIgnoreCase));

        protected int UpdateCount =>
            AllLogs.Count(x =>
                x.ChangeType.Equals("Update", StringComparison.OrdinalIgnoreCase));

        protected int DeleteCount =>
            AllLogs.Count(x =>
                x.ChangeType.Equals("Delete", StringComparison.OrdinalIgnoreCase));

        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {

                    AllLogs = await AuditTrailModel.GetAuditTrailAsync(DatabaseConnection);

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
            IEnumerable<AuditTrailModel> query = AllLogs;

            if (SelectedModule != "All Modules")
            {
                query = query.Where(x =>
                    x.Module.Equals(SelectedModule,
                    StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedChangeType != "All Changes")
            {
                query = query.Where(x =>
                    x.ChangeType.Equals(SelectedChangeType,
                    StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(x =>

                       x.ChangedBy.Contains(SearchTerm,
                            StringComparison.OrdinalIgnoreCase)

                    || x.Module.Contains(SearchTerm,
                            StringComparison.OrdinalIgnoreCase)

                    || x.RecordId.Contains(SearchTerm,
                            StringComparison.OrdinalIgnoreCase)

                    || x.ChangeType.Contains(SearchTerm,
                            StringComparison.OrdinalIgnoreCase));
            }

            FilteredLogs = query
                .OrderByDescending(x => x.ChangedOn)
                .ToList();
        }

        protected string GetChangeTypeBadgeClass(string changeType)
        {
            return changeType.ToLower() switch
            {
                "insert" => "bg-emerald-100 text-emerald-700",
                "update" => "bg-blue-100 text-blue-700",
                "delete" => "bg-rose-100 text-rose-700",
                _ => "bg-slate-100 text-slate-700"
            };
        }
    }
}