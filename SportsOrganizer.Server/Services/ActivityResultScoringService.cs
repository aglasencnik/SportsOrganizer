using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Models;

namespace SportsOrganizer.Server.Services;

public class ActivityResultScoringService
{
    public static List<ActivityResultScoresModel> ScoreActivityResults(List<TeamModel> teams, List<ActivityModel> activities, List<ActivityResultModel> activityResults)
    {
        List<ActivityResultScoresModel> activityResultScores = InitializeActivityResultScores(teams, teams.Count, activities.Count);

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
                int minusScore = 0;

                if (sortedActivityResult.ActivityResult != null) minusScore = teams.Count - (int)sortedActivityResult.Place;

                int index = activityResultScores.FindIndex(x => x.Team.Id == sortedActivityResult.Team.Id);
                activityResultScores[index].Points -= minusScore;
                activityResultScores[index].ActivityPlaces.Add(sortedActivityResult.ActivityResult != null ? (int)sortedActivityResult.Place : 0);
            }
        }

        activityResultScores = ActivityResultSortService.SortActivityResultScores(activityResultScores);

        return activityResultScores;
    }

    private static List<ActivityResultScoresModel> InitializeActivityResultScores(List<TeamModel> teams, int numOfTeams, int numOfActivities)
    {
        List<ActivityResultScoresModel> activityResultScores = new();

        foreach (var team in teams)
        {
            activityResultScores.Add(new ActivityResultScoresModel()
            {
                Team = team,
                Points = numOfTeams * numOfActivities,
                Place = 0,
                ActivityPlaces = new List<int>()
            });
        }

        return activityResultScores;
    }
}
