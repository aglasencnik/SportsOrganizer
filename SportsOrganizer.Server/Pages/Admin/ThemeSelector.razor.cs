using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.Admin;

public class ThemeSelectorBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<ThemeSelector> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
