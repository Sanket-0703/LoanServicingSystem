using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class ForgotPassword : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        // =========================================
        // Page Data
        // =========================================

        public string Email { get; set; } = string.Empty;

        public bool IsLoading { get; set; } = false;

        public bool IsSuccess { get; set; } = false;

        public string? ErrorMessage { get; set; }

        // =========================================
        // Form Actions
        // =========================================

        /// <summary>
        /// Validates the email address and initiates the password
        /// reset request process.
        /// </summary>
        protected async Task HandleResetRequest()
        {
            ErrorMessage = null;

            // Validate email input
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address.";
                return;
            }

            IsLoading = true;

            StateHasChanged();

            try
            {
                if (DatabaseConnection == null)
                    throw new Exception("Database connection is missing.");

                // Check whether the email exists
                var user = await Users.GetUserByEmailAsync(
                    DatabaseConnection,
                    Email);

                // Prevent email enumeration by always showing success
                await Task.Delay(800);

                IsSuccess = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}