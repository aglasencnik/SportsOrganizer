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
    public IStringLocalizer<Activities> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public List<ActivityModel> Activities { get; set; } = new();

    protected override void OnInitialized()
    {
        var result = DbContext.Activities;

        if (result != null) Activities = result.ToList();
    }
}
