using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages;

public class ActivitiesBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<Activities> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected List<ActivityModel> Activities { get; set; } = new();

    protected override void OnInitialized()
    {
        Activities = DbContext.Activities.ToList();
    }
}
