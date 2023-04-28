using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Components.LayoutComponents.HeaderComponent;

public class HeaderBase : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected IStringLocalizer<Header> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    protected string NavbarColor { get; set; }
    protected string ButtonColor { get; set; }
    protected string TextColor { get; set; }
    protected string Title { get; set; }

    protected override void OnInitialized()
    {
        var titleObj = MemoryStorage.GetValue(KeyValueType.Title);

        if (titleObj != null) Title = (string)titleObj;
        else Title = "SportsOrganizer";

        var navbarObj = MemoryStorage.GetValue(KeyValueType.HeaderColor);
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
