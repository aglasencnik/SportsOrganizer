using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Components.MainComponents.HeaderComponent;

public class HeaderBase : ComponentBase
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    public IStringLocalizer<Header> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    public string NavbarColor { get; set; }
    public string ButtonColor { get; set; }
    public string TextColor { get; set; }
    public string Title { get; set; }

    protected override void OnInitialized()
    {
        var titleObj = MemoryStorage.GetValue(KeyValueType.Title);

        if (titleObj != null) Title = (string)titleObj;
        else Title = "SportsOrganizer";

        var navbarObj = MemoryStorage.GetValue(KeyValueType.None);
        if (navbarObj != null) NavbarColor = (string)navbarObj;
        else NavbarColor = Enums.NavbarColor.Primary;

        if (NavbarColor == Enums.NavbarColor.Primary || NavbarColor == Enums.NavbarColor.Dark)
        {
            ButtonColor = "btn btn-secondary";
            TextColor = "text-muted";
        }
        else if (NavbarColor == Enums.NavbarColor.Light)
        {
            ButtonColor = "btn btn-primary";
            TextColor = "text-primary";
        }
    }
}
