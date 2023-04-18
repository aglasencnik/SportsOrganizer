using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.Admin;

public class VersionBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<Version> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
