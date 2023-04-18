using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Pages.Account;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages.User;

public class IndexBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<Index> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }
}
