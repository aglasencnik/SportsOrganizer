using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Components.MainComponents.HeaderComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Components.MainComponents.FooterComponent;

public class FooterBase : ComponentBase
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    public IStringLocalizer<Header> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    public string NavbarColor { get; set; }
    public string CopyrightNotice { get; set; }

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
