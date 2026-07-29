using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages;

public partial class Dashboard
{
    // =========================================
    // Dependency Injection
    // =========================================

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    // =========================================
    // Page Data
    // =========================================

    protected bool IsLoading { get; set; } = true;

    protected string CurrentRole { get; set; } = string.Empty;

    protected string CurrentUserName { get; set; } = string.Empty;

    // =========================================
    // Lifecycle Methods
    // =========================================

    /// <summary>
    /// Retrieves the authenticated user's details and
    /// determines which dashboard should be displayed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        CurrentUserName = user.Identity?.Name ?? string.Empty;

        CurrentRole = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        IsLoading = false;
    }
}