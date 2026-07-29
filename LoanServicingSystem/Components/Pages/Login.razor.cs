using System.Security.Claims;
using CoreData;
using CoreData.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class Login : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // =========================================
        // Cascading Parameters
        // =========================================

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; }

        // =========================================
        // Parameters
        // =========================================

        [SupplyParameterFromForm]
        public LoginViewModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        // =========================================
        // Page State
        // =========================================

        public bool IsLoading { get; set; } = false;

        public string? ErrorMessage { get; set; }

        public string? AlertMessage { get; private set; }

        public string? AlertType { get; private set; }

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Initializes the login form.
        /// </summary>
        protected override void OnInitialized()
        {
            Input ??= new LoginViewModel();
        }

        // =========================================
        // Authentication
        // =========================================

        /// <summary>
        /// Validates user credentials and signs the user into the system.
        /// </summary>
        protected async Task HandleLoginAsync()
        {
            ErrorMessage = null;

            if (DatabaseConnection == null || NavigationManager == null)
                return;

            // Validate user credentials
            var user = await Users.ValidateUserAsync(
                DatabaseConnection,
                Input.Username,
                Input.Password);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            // Verify account status
            if (!user.IsActive)
            {
                ErrorMessage = "Your account has been disabled.";
                return;
            }

            // Build authentication claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // Create authentication cookie and redirect
            if (HttpContext != null)
            {
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                NavigationManager.NavigateTo(
                    ReturnUrl ?? "/dashboard",
                    forceLoad: true);
            }
            else
            {
                ErrorMessage = "An error occurred with the authentication context.";
            }
        }

        // =========================================
        // View Models
        // =========================================

        /// <summary>
        /// Login form model.
        /// </summary>
        public class LoginViewModel
        {
            public string Username { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;
        }
    }
}