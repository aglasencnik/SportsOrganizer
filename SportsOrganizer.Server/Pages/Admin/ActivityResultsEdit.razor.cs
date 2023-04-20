using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Admin;

public class ActivityResultsEditBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<ActivityResultsEdit> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 ||
            user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
