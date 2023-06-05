using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Models.XmlDtos;
using System.Xml.Serialization;

namespace SportsOrganizer.Server.Services;

public class XmlBackupService
{
    public static async Task<(bool, string)> GenerateBackup(ApplicationDbContext db, ILiteDbService<AppSettingsModel> liteDbService)
    {
        try
        {
            var backupModel = new BackupXmlModel();

            backupModel.AppSettings = liteDbService.GetAll().Where(x => x.KeyValueType != Enums.KeyValueType.Database).ToList();
            backupModel.Teams = new();
            backupModel.Users = new();
            backupModel.Activities = new();

            var teamsDb = await db.Teams.ToListAsync();
            var activitiesDb = await db.Activities.ToListAsync();
            var activityResultsDb = await db.ActivityResults.ToListAsync();
            var playerResultsDb = await db.PlayerResults.ToListAsync();
            var usersDb = await db.Users.ToListAsync();
            var userActivitiesDb = await db.UserActivities.ToListAsync();

            foreach (var team in teamsDb ?? Enumerable.Empty<TeamModel>())
            {
                backupModel.Teams.Add(new TeamModelXmlDto
                {
                    Id = team.Id,
                    Name = team.Name
                });
            }

            foreach (var activity in activitiesDb ?? Enumerable.Empty<ActivityModel>())
            {
                var activityResults = new List<ActivityResultModelXmlDto>();

                var activityResultRecords = activityResultsDb.Where(x => x.ActivityId == activity.Id).ToList();

                foreach (var activityResult in activityResultRecords ?? Enumerable.Empty<ActivityResultModel>())
                {
                    var playerResults = new List<PlayerResultModelXmlDto>();

                    var playerResultsRecords = playerResultsDb.Where(x => x.ActivityResultId == activityResult.Id).ToList();

                    foreach (var playerResult in playerResultsRecords ?? Enumerable.Empty<PlayerResultModel>())
                    {
                        playerResults.Add(new PlayerResultModelXmlDto
                        {
                            Id = playerResult.Id,
                            ActivityResultId = activityResult.Id,
                            Result = playerResult.Result
                        });
                    }

                    activityResults.Add(new ActivityResultModelXmlDto
                    {
                        Id = activityResult.Id,
                        Team = backupModel.Teams.FirstOrDefault(x => x.Id == activityResult.TeamId),
                        ActivityId = activityResult.ActivityId,
                        PlayerResults = playerResults,
                        Result = activityResult.Result
                    });
                }

                backupModel.Activities.Add(new ActivityModelXmlDto
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

            foreach (var user in usersDb ?? Enumerable.Empty<UserModel>())
            {
                var userActivities = new List<UserActivityModelXmlDto>();

                var records = userActivitiesDb.Where(x => x.UserId == user.Id).ToList();

                foreach (var record in records ?? Enumerable.Empty<UserActivityModel>())
                {
                    userActivities.Add(new UserActivityModelXmlDto
                    {
                        Id = record.Id,
                        UserId = record.UserId,
                        ActivityId = record.ActivityId,
                        Activity = backupModel.Activities.FirstOrDefault(x => x.Id == record.ActivityId) ?? new ActivityModelXmlDto()
                    });
                }

                backupModel.Users.Add(new UserModelXmlDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password,
                    UserType = user.UserType,
                    AssignedActivities = userActivities
                });
            }

            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(BackupXmlModel));
            serializer.Serialize(stringWriter, backupModel);
            return (true, stringWriter.ToString());
        }
        catch
        {
            return (false, string.Empty);
        }
    }

    public static async Task<bool> RestoreApplication(ApplicationDbContext db, ILiteDbService<AppSettingsModel> liteDbService, string data)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BackupXmlModel));
            var backupModel = new BackupXmlModel();
            using (StringReader reader = new StringReader(data))
            {
                backupModel = (BackupXmlModel)serializer.Deserialize(reader);
            }

            var oldTeamsDb = await db.Teams.ToListAsync();
            var oldActivitiesDb = await db.Activities.ToListAsync();
            var oldActivityResultsDb = await db.ActivityResults.ToListAsync();
            var oldPlayerResultsDb = await db.PlayerResults.ToListAsync();
            var oldUsersDb = await db.Users.ToListAsync();
            var oldUserActivitiesDb = await db.UserActivities.ToListAsync();
            var oldLiteDbResults = liteDbService.GetAll().ToList();

            if (oldPlayerResultsDb != null && oldPlayerResultsDb.Count != 0)
                db.PlayerResults.RemoveRange(oldPlayerResultsDb);

            if (oldActivityResultsDb != null && oldActivityResultsDb.Count != 0)
                db.ActivityResults.RemoveRange(oldActivityResultsDb);

            if (oldUserActivitiesDb != null && oldUserActivitiesDb.Count != 0)
                db.UserActivities.RemoveRange(oldUserActivitiesDb);

            if (oldActivitiesDb != null && oldActivitiesDb.Count != 0)
                db.Activities.RemoveRange(oldActivitiesDb);

            if (oldTeamsDb != null && oldTeamsDb.Count != 0)
                db.Teams.RemoveRange(oldTeamsDb);

            if (oldUsersDb != null && oldUsersDb.Count != 0)
                db.Users.RemoveRange(oldUsersDb);

            foreach (var liteDbResult in oldLiteDbResults.Where(x => x.KeyValueType != Enums.KeyValueType.Database) ?? Enumerable.Empty<AppSettingsModel>())
                liteDbService.Delete(liteDbResult.Id);

            // Insert into LiteDB
            foreach (var liteDbResult in backupModel.AppSettings ?? Enumerable.Empty<AppSettingsModel>())
            {
                liteDbService.Insert(new AppSettingsModel
                {
                    KeyValueType = liteDbResult.KeyValueType,
                    Value = liteDbResult.Value
                });
            }

            // Insert into Teams
            var teams = new List<TeamModel>();
            foreach (var team in backupModel.Teams ?? Enumerable.Empty<TeamModelXmlDto>())
            {
                teams.Add(new TeamModel
                {
                    Name = team.Name
                });
            }
            await db.Teams.AddRangeAsync(teams);
            await db.SaveChangesAsync();

            // Insert into Activities, ActivityResults, PlayerResults
            foreach (var activity in backupModel.Activities ?? Enumerable.Empty<ActivityModelXmlDto>())
            {
                var newActivity = new ActivityModel
                {
                    ActivityNumber = activity.ActivityNumber,
                    Title = activity.Title,
                    Location = activity.Location,
                    Rules = activity.Rules,
                    Props = activity.Props,
                    ActivityType = activity.ActivityType,
                    OrderType = activity.OrderType,
                    NumberOfPlayers = activity.NumberOfPlayers
                };
                await db.Activities.AddAsync(newActivity);
                await db.SaveChangesAsync();

                foreach (var activityResult in activity.ActivityResults ?? Enumerable.Empty<ActivityResultModelXmlDto>())
                {
                    var team = teams.FirstOrDefault(x => x.Name == activityResult.Team.Name);

                    var newActivityResult = new ActivityResultModel
                    {
                        ActivityId = newActivity.Id,
                        TeamId = team.Id,
                        Result = activityResult.Result
                    };

                    await db.ActivityResults.AddAsync(newActivityResult);
                    await db.SaveChangesAsync();

                    foreach (var playerResult in activityResult.PlayerResults ?? Enumerable.Empty<PlayerResultModelXmlDto>())
                    {
                        var newPlayerResult = new PlayerResultModel
                        {
                            ActivityResultId = newActivityResult.Id,
                            Result = playerResult.Result
                        };

                        await db.PlayerResults.AddAsync(newPlayerResult);
                        await db.SaveChangesAsync();
                    }
                }
            }

            // Insert into Users, UserActivities
            foreach (var user in backupModel.Users ?? Enumerable.Empty<UserModelXmlDto>())
            {
                var newUser = new UserModel
                {
                    Username = user.Username,
                    Password = user.Password,
                    UserType = user.UserType
                };
                await db.Users.AddAsync(newUser);
                await db.SaveChangesAsync();

                foreach (var userActivity in user.AssignedActivities ?? Enumerable.Empty<UserActivityModelXmlDto>())
                {
                    var activity = db.Activities.FirstOrDefault(x => x.ActivityNumber == userActivity.Activity.ActivityNumber
                                                                && x.Title == userActivity.Activity.Title
                                                                && x.Location == userActivity.Activity.Location
                                                                && x.Props == userActivity.Activity.Props
                                                                && x.ActivityType == userActivity.Activity.ActivityType);

                    if (activity != null)
                    {
                        var newUserActivity = new UserActivityModel
                        {
                            UserId = newUser.Id,
                            ActivityId = activity.Id
                        };

                        await db.UserActivities.AddAsync(newUserActivity);
                        await db.SaveChangesAsync();
                    }
                }

            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
