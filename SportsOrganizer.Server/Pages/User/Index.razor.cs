using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.User;

public class IndexBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<Index> Localizer { get; set; }

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

        if (user.Identities.Count() == 0) NavigationManager.NavigateTo("/");
    }
}
