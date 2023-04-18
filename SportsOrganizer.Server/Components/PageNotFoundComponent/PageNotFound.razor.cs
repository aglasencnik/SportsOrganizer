using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace SportsOrganizer.Server.Components.PageNotFoundComponent;

public class PageNotFoundBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<PageNotFound> Localizer { get; set; }
}
