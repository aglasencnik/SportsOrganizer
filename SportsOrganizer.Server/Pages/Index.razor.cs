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
    public IStringLocalizer<Index> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public List<TeamModel> Teams { get; set; } = new();
    public List<ActivityModel> Activities { get; set; } = new();
    public List<ActivityResultModel> ActivityResults { get; set; } = new();
    public List<ActivityResultScoresModel> ActivityResultScores { get; set; } = new();
    public string TableColor { get; set; }
    public int NumberOfActivities { get; set; }
    public string Homepage { get; set; }

    protected override void OnInitialized()
    {
        Teams = DbContext.Teams.ToList();
        Activities = DbContext.Activities.ToList();
        ActivityResults = DbContext.ActivityResults.ToList();

        NumberOfActivities = Activities.Count();

        var scoreObj = MemoryStorage.GetValue(KeyValueType.ScoringType);
        ScoringType scoringType = (scoreObj != null) ? (ScoringType)scoreObj : ScoringType.Ascending;

        ActivityResultScores = ActivityResultScoringService.ScoreActivityResults(Teams, Activities, ActivityResults, scoringType);

        var colorObj = MemoryStorage.GetValue(KeyValueType.FinalScoresTableColor);

        if (colorObj != null) TableColor = (string)colorObj;
        else TableColor = Enums.TableColor.Default;

        var homepageObj = MemoryStorage.GetValue(KeyValueType.Homepage);

        if (homepageObj != null) Homepage = (string)homepageObj;
        else Homepage = string.Empty;
    }
}
