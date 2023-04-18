using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Pages.Account;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.User;

public class InsertActivityResultsSelectorBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<InsertActivityResultsSelector> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
