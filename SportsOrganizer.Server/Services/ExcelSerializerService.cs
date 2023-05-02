using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;

namespace SportsOrganizer.Server.Services;

public class ExcelSerializerService
{
    public static async Task<(bool, byte[])> SerializeTeamsToExcel(ApplicationDbContext db)
    {
        try
        {
            var teamsDb = await db.Teams.ToListAsync();

            using var workbook = new XLWorkbook();
            var teamsWorksheet = workbook.AddWorksheet("Teams");
            teamsWorksheet.Author = "SportsOrganizer";

            int rowCounter = 1;

            teamsWorksheet.Cell($"A{rowCounter}").Value = "Id";
            teamsWorksheet.Cell($"B{rowCounter}").Value = "Name";
            rowCounter++;

            foreach (var team in teamsDb)
            {
                teamsWorksheet.Cell($"A{rowCounter}").Value = team.Id;
                teamsWorksheet.Cell($"B{rowCounter}").Value = team.Name;
                rowCounter++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return (true, stream.ToArray());
            }
        }
        catch
        {
            return (false, null);
        }
    }

    public static async Task<(bool, byte[])> SerializeActivitiesToExcel(ApplicationDbContext db)
    {
        try
        {
            var activitiesDb = await db.Activities.OrderBy(x => x.ActivityNumber).ToListAsync();
            var activityResultsDb = await db.ActivityResults.ToListAsync();
            var playerResultsDb = await db.PlayerResults.ToListAsync();
            var teamsDb = await db.Teams.ToListAsync();

            using var workbook = new XLWorkbook();

            foreach (var activity in activitiesDb)
            {
                var worksheet = workbook.AddWorksheet($"{activity.ActivityNumber}. Activity");
                worksheet.Author = "SportsOrganizer";

                int rowCounter = 1;

                worksheet.Cell($"A{rowCounter++}").Value = "Id";
                worksheet.Cell($"A{rowCounter++}").Value = "Activity Number";
                worksheet.Cell($"A{rowCounter++}").Value = "Title";
                worksheet.Cell($"A{rowCounter++}").Value = "Location";
                worksheet.Cell($"A{rowCounter++}").Value = "Rules";
                worksheet.Cell($"A{rowCounter++}").Value = "Props";
                worksheet.Cell($"A{rowCounter++}").Value = "Activity Type";
                worksheet.Cell($"A{rowCounter++}").Value = "Order Type";
                worksheet.Cell($"A{rowCounter++}").Value = "Number of Players";

                rowCounter = 1;
                worksheet.Cell($"B{rowCounter++}").Value = activity.Id;
                worksheet.Cell($"B{rowCounter++}").Value = activity.ActivityNumber;
                worksheet.Cell($"B{rowCounter++}").Value = activity.Title;
                worksheet.Cell($"B{rowCounter++}").Value = activity.Location;
                worksheet.Cell($"B{rowCounter++}").Value = activity.Rules;
                worksheet.Cell($"B{rowCounter++}").Value = activity.Props;
                worksheet.Cell($"B{rowCounter++}").Value = activity.ActivityType.ToString();
                worksheet.Cell($"B{rowCounter++}").Value = activity.OrderType.ToString();
                worksheet.Cell($"B{rowCounter++}").Value = activity.NumberOfPlayers;

                char col = 'D';
                rowCounter = 1;
                worksheet.Cell($"{col}{rowCounter}").Value = "Team Name";
                if (activity.NumberOfPlayers > 1)
                {
                    col++;
                    for (int i = 1; i <= activity.NumberOfPlayers; i++)
                    {
                        worksheet.Cell($"{col}{rowCounter}").Value = $"{i}. Player";
                        col++;
                    }
                }
                else col++;
                worksheet.Cell($"{col}{rowCounter}").Value = "Final Result";

                var activityResults = activityResultsDb.Where(x => x.ActivityId == activity.Id).ToList();

                if (activityResults != null && activityResults.Count != 0)
                {
                    rowCounter = 2;
                    foreach (var activityResult in activityResults)
                    {
                        var team = teamsDb.FirstOrDefault(x => x.Id == activityResult.TeamId);

                        col = 'D';
                        worksheet.Cell($"{col}{rowCounter}").Value = (team != null) ? team.Name : string.Empty;
                        if (activity.NumberOfPlayers > 1)
                        {
                            var playerResults = playerResultsDb.Where(x => x.ActivityResultId == activityResult.Id).ToList();
                            if (playerResults != null && playerResults.Count != 0)
                            {
                                col++;
                                for (int i = 0; i < activity.NumberOfPlayers; i++)
                                {
                                    if (playerResults.Count >= i + 1)
                                    {
                                        worksheet.Cell($"{col}{rowCounter}").Value = playerResults[i].Result;
                                    }
                                    else
                                    {
                                        worksheet.Cell($"{col}{rowCounter}").Value = string.Empty;
                                    }
                                    col++;
                                }
                            }
                            else
                            {
                                col++;
                                for (int i = 1; i <= activity.NumberOfPlayers; i++)
                                {
                                    worksheet.Cell($"{col}{rowCounter}").Value = string.Empty;
                                    col++;
                                }
                            }
                        }
                        else col++;
                        worksheet.Cell($"{col}{rowCounter}").Value = activityResult.Result;

                        rowCounter++;
                    }
                }
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return (true, stream.ToArray());
            }
        }
        catch
        {
            return (false, null);
        }
    }

    public static async Task<(bool, byte[])> SerializeUsersToExcel(ApplicationDbContext db)
    {
        try
        {
            var usersDb = await db.Users.ToListAsync();
            var userActivitiesDb = await db.UserActivities.ToListAsync();

            using var workbook = new XLWorkbook();
            var usersWorksheet = workbook.AddWorksheet("Users");
            usersWorksheet.Author = "SportsOrganizer";

            int rowCounter = 1;

            usersWorksheet.Cell($"A{rowCounter}").Value = "Id";
            usersWorksheet.Cell($"B{rowCounter}").Value = "Username";
            usersWorksheet.Cell($"C{rowCounter}").Value = "Password";
            usersWorksheet.Cell($"D{rowCounter}").Value = "User Type";
            usersWorksheet.Cell($"E{rowCounter}").Value = "Assigned Activities";
            rowCounter++;

            foreach (var user in usersDb)
            {
                string assignedActivities = string.Empty;

                if (user.UserType == Data.Enums.UserType.User)
                {
                    var userActivities = userActivitiesDb.Where(x => x.UserId == user.Id).ToList();

                    if (userActivities != null && userActivities.Count != 0)
                    {
                        var activityIds = userActivities.Select(x => x.ActivityId);
                        var activities = db.Activities.Where(x => activityIds.Contains(x.Id)).ToList();

                        if (activities != null && activities.Count != 0)
                        {
                            assignedActivities = string.Join(", ", activities.OrderBy(o => o.ActivityNumber).Select(o => o.ActivityNumber));
                        }
                    }
                }
                else if (user.UserType == Data.Enums.UserType.Admin)
                {
                    assignedActivities = "All";
                }

                usersWorksheet.Cell($"A{rowCounter}").Value = user.Id;
                usersWorksheet.Cell($"B{rowCounter}").Value = user.Username;
                usersWorksheet.Cell($"C{rowCounter}").Value = user.Password;
                usersWorksheet.Cell($"D{rowCounter}").Value = user.UserType.ToString();
                usersWorksheet.Cell($"E{rowCounter}").Value = assignedActivities;
                rowCounter++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return (true, stream.ToArray());
            }
        }
        catch
        {
            return (false, null);
        }
    }
}
