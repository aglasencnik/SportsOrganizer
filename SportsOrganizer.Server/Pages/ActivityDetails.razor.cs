using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages;

public class ActivityDetailsBase : ComponentBase
{
    [Parameter]
    public int ActivityId { get; set; }

    [Inject]
    protected IStringLocalizer<ActivityDetails> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected ActivityModel Activity { get; set; } = new();
    protected List<TeamModel> Teams { get; set; } = new();
    protected List<ActivityResultModel> ActivityResults { get; set; } = new();
    protected List<PlayerResultModel> PlayerResults { get; set; } = new();
    protected string TableColor { get; set; }

    protected override void OnInitialized()
    {
        if (ActivityId != 0)
        {
            Activity = DbContext.Activities.FirstOrDefault(x => x.Id == ActivityId);

            if (Activity != null)
            {
                Teams = DbContext.Teams.ToList();
                ActivityResults = DbContext.ActivityResults.Where(x => x.ActivityId == ActivityId).ToList();
                
                if (Activity.NumberOfPlayers > 1 && ActivityResults != null && ActivityResults.Count != 0)
                    PlayerResults = DbContext.PlayerResults
                        .Where(pr => ActivityResults
                            .Select(ar => ar.Id)
                            .Contains(pr.ActivityResultId))
                        .ToList();

                var colorObj = MemoryStorage.GetValue(KeyValueType.SingleActivityTableColor);
                TableColor = (colorObj != null) ? (string)colorObj : Enums.TableColor.Default;
            }
            else NavigationManager.NavigateTo("/");
        }
        else NavigationManager.NavigateTo("/");
    }
}
