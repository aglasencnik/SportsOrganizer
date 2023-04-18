using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.Admin;

public class BasicInfoEditBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<BasicInfoEdit> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
