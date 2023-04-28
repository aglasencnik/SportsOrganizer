using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Components.LayoutComponents.FooterComponent;

public class FooterBase : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected IStringLocalizer<Footer> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    protected string NavbarColor { get; set; }
    protected string CopyrightNotice { get; set; }

    protected override void OnInitialized()
    {
        var copyrightObj = MemoryStorage.GetValue(KeyValueType.CopyrightNotice);

        if (copyrightObj != null) CopyrightNotice = (string)copyrightObj;
        else CopyrightNotice = "SportsOrganizer";

        var navbarObj = MemoryStorage.GetValue(KeyValueType.FooterColor);
        if (navbarObj != null) NavbarColor = (string)navbarObj;
        else NavbarColor = Enums.NavbarColor.Dark;
    }
}
