using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages
{
    public partial class Notifications : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public bool IsLoading { get; set; } = true;

        public List<Notification> NotificationList { get; set; } = new();

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        public Guid CurrentUserId { get; private set; }



        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();


            if (authState.User.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = Users.GetCurrentUserId(authState.User);
                await LoadNotifications();
            }

        }

        private async Task LoadNotifications()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    NotificationList =
                        await Notification.GetAllAsync(
                            DatabaseConnection,
                            CurrentUserId);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task MarkAsRead(Guid id)
        {
            if (DatabaseConnection == null)
                return;

            await Notification.MarkAsReadAsync(
                DatabaseConnection,
                id);

            await LoadNotifications();
        }

        private async Task MarkAllAsRead()
        {
            if (DatabaseConnection == null)
                return;

            await Notification.MarkAllAsReadAsync(
                DatabaseConnection,
                CurrentUserId);

            await LoadNotifications();
        }
    }
}