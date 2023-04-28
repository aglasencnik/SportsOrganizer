using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Components.PageNotFoundComponent;

public class PageNotFoundBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<PageNotFound> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }
}
