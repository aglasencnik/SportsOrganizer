using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Components.PageNotFoundComponent;

public class PageNotFoundBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<PageNotFound> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
