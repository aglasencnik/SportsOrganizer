using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;

namespace SportsOrganizer.Server.Services;

public class ActivityResultScoringService
{
    public static List<ActivityResultScoresModel> ScoreActivityResults(List<TeamModel> teams, List<ActivityModel> activities, List<ActivityResultModel> activityResults, ScoringType scoringType)
    {
        List<ActivityResultScoresModel> activityResultScores = InitializeActivityResultScores(teams, teams.Count, activities.Count, scoringType);

        foreach (var activity in activities)
        {
            List<ActivityResultSortModel> sortedActivityResults = new();

            foreach (var team in teams)
            {
                var activityResult = activityResults.FirstOrDefault(x => x.TeamId == team.Id && x.ActivityId == activity.Id);

                sortedActivityResults.Add(new ActivityResultSortModel()
                {
                    Team = team,
                    Activity = activity,
                    ActivityResult = activityResult
                });
            }

            sortedActivityResults = ActivityResultSortService.SortActivityResults(sortedActivityResults, activity.OrderType);

            foreach (var sortedActivityResult in sortedActivityResults)
            {
                int index = activityResultScores.FindIndex(x => x.Team.Id == sortedActivityResult.Team.Id);

                if (scoringType == ScoringType.Ascending)
                    activityResultScores[index].Points += sortedActivityResult.Place;
                else if (scoringType == ScoringType.Descending) 
                    activityResultScores[index].Points -= sortedActivityResult.Place;

                activityResultScores[index].ActivityPlaces.Add(sortedActivityResult.ActivityResult != null ? sortedActivityResult.Place : 0);
            }
        }

        activityResultScores = ActivityResultSortService.SortActivityResultScores(activityResultScores, scoringType);

        return activityResultScores;
    }

    private static List<ActivityResultScoresModel> InitializeActivityResultScores(List<TeamModel> teams, int numOfTeams, int numOfActivities, ScoringType scoringType)
    {
        List<ActivityResultScoresModel> activityResultScores = new();

        foreach (var team in teams)
        {
            activityResultScores.Add(new ActivityResultScoresModel()
            {
                Team = team,
                Points = (scoringType == ScoringType.Ascending) ? 0 : numOfTeams * numOfActivities,
                Place = 0,
                ActivityPlaces = new List<int>()
            });
        }

        return activityResultScores;
    }
}
