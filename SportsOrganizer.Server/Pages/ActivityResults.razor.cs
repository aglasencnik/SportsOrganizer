using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Pages;

public class ActivityResultsBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<ActivityResults> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected List<ActivityModel> Activities { get; set; } = new();
    protected List<TeamModel> Teams { get; set; } = new();
    protected List<ActivityResultModel> ActivityResults { get; set; } = new();
    protected List<PlayerResultModel> PlayerResults { get; set; } = new();
    protected string TableColor { get; set; }

    protected override void OnInitialized()
    {
        Activities = DbContext.Activities.ToList();

        if (Activities != null && Activities.Count != 0)
        {
            Teams = DbContext.Teams.ToList();
            ActivityResults = DbContext.ActivityResults.ToList();
            PlayerResults = DbContext.PlayerResults.ToList();

            var colorObj = MemoryStorage.GetValue(KeyValueType.SingleActivityTableColor);

            if (colorObj != null) TableColor = (string)colorObj;
            else TableColor = Enums.TableColor.Default;
        }
    }
}
