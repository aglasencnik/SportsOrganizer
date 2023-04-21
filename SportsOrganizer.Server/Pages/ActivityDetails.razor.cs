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
    public IStringLocalizer<ActivityDetails> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public ActivityModel Activity { get; set; } = new();
    public List<TeamModel> Teams { get; set; } = new();
    public List<ActivityResultModel> ActivityResults { get; set; } = new();
    public List<PlayerResultModel> PlayerResults { get; set; } = new();
    public string TableColor { get; set; }

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

                var colorObj = MemoryStorage.GetValue(KeyValueType.TableColor);

                if (colorObj != null) TableColor = (string)colorObj;
                else TableColor = Enums.TableColor.Default;
            }
            else NavigationManager.NavigateTo("/");
        }
        else NavigationManager.NavigateTo("/");
    }
}
