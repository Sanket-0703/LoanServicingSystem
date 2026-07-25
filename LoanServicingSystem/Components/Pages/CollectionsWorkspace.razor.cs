using CoreData;
using Microsoft.AspNetCore.Components;
using static CoreData.Servicing.RepaymentSchedule;

namespace LoanServicingSystem.Components.Pages
{
    public partial class CollectionsWorkspace : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;
        public CollectionsData WorkspaceData { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    WorkspaceData = await CollectionsData.GetCollectionsWorkspaceAsync(DatabaseConnection, DateTime.Today);
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected string GetDpdBadgeClass(int daysPastDue)
        {
            if (daysPastDue >= 90) return "bg-rose-100 text-rose-800"; // Severe Default
            if (daysPastDue >= 30) return "bg-red-100 text-red-800";   // High Risk
            return "bg-amber-100 text-amber-800";                      // Early Delinquency
        }
    }
}