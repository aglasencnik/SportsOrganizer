using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Models.XmlDtos;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SportsOrganizer.Server.Services;

public class XmlSerializerService
{
    public static async Task<(bool, string)> SerializeTeamsToXml(ApplicationDbContext db)
    {
        try
        {
            var teamsDb = await db.Teams.ToListAsync();

            var serializedTeams = new List<TeamModelXmlDto>();
            foreach (var team in teamsDb ?? Enumerable.Empty<TeamModel>())
            {
                serializedTeams.Add(new TeamModelXmlDto
                {
                    Id = team.Id,
                    Name = team.Name
                });
            }

            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(List<TeamModelXmlDto>));
            serializer.Serialize(stringWriter, serializedTeams);
            return (true, stringWriter.ToString());
        }
        catch
        {
            return (false, string.Empty);
        }
    }

    public static async Task<(bool, string)> SerializeActivitiesToXml(ApplicationDbContext db)
    {
        try
        {
            var activitiesDb = await db.Activities.ToListAsync();
            var activityResultsDb = await db.ActivityResults.ToListAsync();
            var playerResultsDb = await db.PlayerResults.ToListAsync();

            var serializedActivities = new List<ActivityModelXmlDto>();
            foreach (var activity in activitiesDb ?? Enumerable.Empty<ActivityModel>())
            {
                var activityResults = new List<ActivityResultModelXmlDto>();
                foreach (var activityResult in activityResultsDb.Where(x => x.ActivityId == activity.Id) ?? Enumerable.Empty<ActivityResultModel>())
                {
                    var playerResults = new List<PlayerResultModelXmlDto>();
                    foreach (var playerResult in playerResultsDb.Where(x => x.ActivityResultId == activityResult.Id) ?? Enumerable.Empty<PlayerResultModel>())
                    {
                        playerResults.Add(new PlayerResultModelXmlDto
                        {
                            Id = playerResult.Id,
                            ActivityResultId = playerResult.ActivityResultId,
                            Result = playerResult.Result
                        });
                    }

                    activityResults.Add(new ActivityResultModelXmlDto
                    {
                        Id = activityResult.Id,
                        ActivityId = activityResult.TeamId,
                        PlayerResults = playerResults,
                        Result = activityResult.Result
                    });
                }

                serializedActivities.Add(new ActivityModelXmlDto
                {
                    Id = activity.Id,
                    ActivityNumber = activity.ActivityNumber,
                    Title = activity.Title,
                    Location = activity.Location,
                    Rules = activity.Rules,
                    Props = activity.Props,
                    ActivityType = activity.ActivityType,
                    OrderType = activity.OrderType,
                    NumberOfPlayers = activity.NumberOfPlayers,
                    ActivityResults = activityResults
                });
            }

            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ActivityModelXmlDto>));
            serializer.Serialize(stringWriter, serializedActivities);
            return (true, stringWriter.ToString());
        }
        catch
        {
            return (false, string.Empty);
        }
    }

    public static async Task<(bool, string)> SerializeUsersToXml(ApplicationDbContext db)
    {
        try
        {
            var usersDb = await db.Users.ToListAsync();
            var userActivitiesDb = await db.UserActivities.ToListAsync();

            var serializedUsers = new List<UserModelXmlDto>();
            foreach (var user in usersDb ?? Enumerable.Empty<UserModel>())
            {
                var assignedActivities = new List<UserActivityModelXmlDto>();

                foreach (var assignedActivity in userActivitiesDb.Where(x => x.UserId == user.Id) ?? Enumerable.Empty<UserActivityModel>())
                {
                    assignedActivities.Add(new UserActivityModelXmlDto
                    {
                        Id = assignedActivity.Id,
                        UserId = user.Id,
                        ActivityId = assignedActivity.Id
                    });
                }

                serializedUsers.Add(new UserModelXmlDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password,
                    UserType = user.UserType,
                    AssignedActivities = assignedActivities
                });
            }

            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(List<UserModelXmlDto>));
            serializer.Serialize(stringWriter, serializedUsers);
            return (true, stringWriter.ToString());
        }
        catch
        {
            return (false, string.Empty);
        }
    }
}
