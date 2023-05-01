using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;

namespace SportsOrganizer.Server.Pages;

public class IndexBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<Index> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected List<TeamModel> Teams { get; set; } = new();
    protected List<ActivityModel> Activities { get; set; } = new();
    protected List<ActivityResultModel> ActivityResults { get; set; } = new();
    protected List<ActivityResultScoresModel> ActivityResultScores { get; set; } = new();
    protected string TableColor { get; set; }
    protected int NumberOfActivities { get; set; }
    protected string Homepage { get; set; }

    protected override void OnInitialized()
    {
        Teams = DbContext.Teams.ToList();
        Activities = DbContext.Activities.ToList();
        ActivityResults = DbContext.ActivityResults.ToList();

        NumberOfActivities = Activities.Count();

        var scoreObj = MemoryStorage.GetValue(KeyValueType.ScoringType);
        ScoringType scoringType = (scoreObj == null 
            || !Enum.TryParse(scoreObj.ToString(), out ScoringType result))
            ? ScoringType.Ascending
            : result;

        ActivityResultScores = ActivityResultScoringService.ScoreActivityResults(Teams, Activities, ActivityResults, scoringType);

        var colorObj = MemoryStorage.GetValue(KeyValueType.FinalScoresTableColor);
        TableColor = (colorObj != null) ? (string)colorObj : Enums.TableColor.Default;

        var homepageObj = MemoryStorage.GetValue(KeyValueType.Homepage);
        Homepage = (homepageObj != null) ? (string)homepageObj : string.Empty;
    }
}
