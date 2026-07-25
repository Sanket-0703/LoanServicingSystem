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
        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; }

        [SupplyParameterFromForm]
        public LoginViewModel Input { get; set; } = new();
        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }

        public bool IsLoading { get; set; } = false;
        public string? AlertMessage { get; private set; }
        public string? AlertType { get; private set; }

        protected override void OnInitialized()
        {
            Input ??= new LoginViewModel();
        }

        protected async Task HandleLoginAsync()
        {
            ErrorMessage = null;

            if (DatabaseConnection == null || NavigationManager == null) return;

            // 1. Check database for a matching user
            var user = await Users.ValidateUserAsync(DatabaseConnection, Input.Username, Input.Password);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            if (!user.IsActive)
            {
                ErrorMessage = "Your account has been disabled.";
                return;
            }

            // 2. Build the Security Claims (Your VIP Pass)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleName) // This tells the [Authorize] tag you are an Admin!
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 3. Bake the Cookie and Redirect
            if (HttpContext != null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                NavigationManager.NavigateTo(ReturnUrl ?? "/dashboard", forceLoad: true);
                // Redirect back to the page they tried to access, or default to Dashboard
                NavigationManager.NavigateTo(ReturnUrl ?? "/dashboard");
            }
            else
            {
                ErrorMessage = "An error occurred with the authentication context.";
            }
        }

        public class LoginViewModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}