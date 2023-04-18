using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Pages.Account;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.User;

public class InsertActivityResultsBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<InsertActivityResults> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
