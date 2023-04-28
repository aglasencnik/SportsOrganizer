using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Account;

public class LoginBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<Login> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [Inject] 
    protected AuthenticatorService AuthService { get; set; }

    [CascadingParameter] 
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected IToastService ToastService { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected string Username { get; set; }
    protected string Password { get; set; }
    protected string ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() > 0)
        {
            if (user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value == UserType.Admin.ToString())
            {
                NavigationManager.NavigateTo("/Admin/");
            }
            else if (user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value == UserType.User.ToString())
            {
                NavigationManager.NavigateTo("/User/");
            }
        }
    }

    protected async Task TryLogin()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "";

                var userSearch = DbContext.Users.FirstOrDefault(x => x.Username == Username);

                if (userSearch != null)
                {
                    if (userSearch.Password == Password)
                    {
                        await AuthService.LoginAsync(Username, Password);

                        ToastService.ShowSuccess(Localizer["SuccessMessage"]);

                        if (userSearch.UserType == UserType.Admin)
                        {
                            NavigationManager.NavigateTo("/Admin/");
                        }
                        else if (userSearch.UserType == UserType.User)
                        {
                            NavigationManager.NavigateTo("/User/");
                        }
                    }
                    else ErrorMessage = Localizer["IncorrectPasswordError"];
                }
                else ErrorMessage = Localizer["UserNotFoundError"];
            }
            else ToastService.ShowWarning(Localizer["WarningMessage"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorMessage"]);
        }
    }
}
