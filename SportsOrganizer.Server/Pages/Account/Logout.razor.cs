using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Pages.Account;

public class LogoutBase : ComponentBase
{
    [Inject]
    protected AuthenticatorService AuthenticatorService { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() > 0)
        {
            await AuthenticatorService.LogoutAsync();
            NavigationManager.NavigateTo("/Account/Login", true);
        }
        else
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
