using CoreData;
using Microsoft.AspNetCore.Components;

using static CoreData.Servicing.RepaymentSchedule;

namespace LoanServicingSystem.Components.Pages
{
    public partial class CollectionsWorkspace : ComponentBase
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

        public CollectionsData WorkspaceData { get; set; } = new();

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads the collections workspace data when the page is initialized.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    WorkspaceData = await CollectionsData.GetCollectionsWorkspaceAsync(
                        DatabaseConnection,
                        DateTime.Today);
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        // =========================================
        // UI Helper Methods
        // =========================================

        /// <summary>
        /// Returns the badge style based on the Days Past Due (DPD) value.
        /// </summary>
        protected string GetDpdBadgeClass(int daysPastDue)
        {
            if (daysPastDue >= 90)
                return "bg-rose-100 text-rose-800";     // Severe Default

            if (daysPastDue >= 30)
                return "bg-red-100 text-red-800";      // High Risk

            return "bg-amber-100 text-amber-800";      // Early Delinquency
        }
    }
}