using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public bool IsLoading { get; set; } = false;
        public string? AlertMessage { get; private set; }
        public string? AlertType { get; private set; }

        private async Task HandleLogin()
        {
            AlertMessage = null;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                AlertType = "error";
                AlertMessage = "Please enter both username and password.";
                return;
            }

            IsLoading = true;
            StateHasChanged();

            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection is missing.");

                // Fetch user from DB using Dapper (Defined in Step 2)
                var user = await Users.GetUserByUsernameAsync(DatabaseConnection, Username);

                if (user == null || !user.IsActive)
                {
                    AlertType = "error";
                    AlertMessage = "Invalid credentials or account is inactive.";
                    return;
                }

                // TODO: Implement actual password hashing verification here
                // Example: var isPasswordValid = PasswordHasher.VerifyHashedPassword(user.PasswordHash, Password);
                // Doing this to make flow work without implementing hashing for now
                if (user.PasswordHash != Password)
                {
                    AlertType = "error";
                    AlertMessage = "Invalid credentials.";
                    return;
                }

                // Success! Route to dashboard
                NavigationManager?.NavigateTo("/dashboard");
            }
            catch (Exception ex)
            {
                AlertType = "error";
                AlertMessage = $"An error occurred during login: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}