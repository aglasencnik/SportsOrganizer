using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.Admin;

public class ActivityResultsEditBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<ActivityResultsEdit> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
