using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoanServicingSystem.Components.Pages;

public partial class Dashboard
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    protected bool IsLoading = true;

    protected string CurrentRole = "";

    protected string CurrentUserName = "";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        CurrentUserName = user.Identity?.Name ?? "";

        CurrentRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "";

        IsLoading = false;
    }
}