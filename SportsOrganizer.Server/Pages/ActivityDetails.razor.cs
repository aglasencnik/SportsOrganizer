using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages;

public class ActivityDetailsBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<ActivityDetails> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
