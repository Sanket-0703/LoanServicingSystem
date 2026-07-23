using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class ForgotPassword : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        public string Email { get; set; } = string.Empty; // Note: Ensure standard auto-property syntax 'get; set;'
        public bool IsLoading { get; set; } = false;
        public bool IsSuccess { get; set; } = false;
        public string? ErrorMessage { get; set; }

        protected async Task HandleResetRequest()
        {
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address.";
                return;
            }

            IsLoading = true;
            StateHasChanged();

            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection is missing.");

                // Check if user exists in the database by email
                var user = await Users.GetUserByEmailAsync(DatabaseConnection, Email);

                // Even if user isn't found, standard security practice is to show the success message 
                // to prevent email enumeration attacks, but we query the database to validate the flow.
                await Task.Delay(800); // Simulate network request

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