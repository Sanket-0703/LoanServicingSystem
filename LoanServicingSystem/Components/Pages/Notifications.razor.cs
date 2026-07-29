using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages
{
    public partial class Notifications : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        // =========================================
        // Page State
        // =========================================

        public bool IsLoading { get; set; } = true;

        public Guid CurrentUserId { get; private set; }

        public List<Notification> NotificationList { get; set; } = new();

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads notifications for the currently logged-in user.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            if (authState.User.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = Users.GetCurrentUserId(authState.User);

                await LoadNotifications();
            }
        }

        // =========================================
        // Data Loading
        // =========================================

        /// <summary>
        /// Retrieves all notifications for the current user.
        /// </summary>
        private async Task LoadNotifications()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    NotificationList = await Notification.GetAllAsync(
                        DatabaseConnection,
                        CurrentUserId);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        // =========================================
        // Notification Actions
        // =========================================

        /// <summary>
        /// Marks a single notification as read.
        /// </summary>
        private async Task MarkAsRead(Guid id)
        {
            if (DatabaseConnection == null)
                return;

            await Notification.MarkAsReadAsync(
                DatabaseConnection,
                id);

            await LoadNotifications();
        }

        /// <summary>
        /// Marks all notifications for the current user as read.
        /// </summary>
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