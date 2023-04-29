using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.AdminComponents.AdminActivityResultEditModalComponent;

public class AdminActivityResultEditModalBase : ComponentBase
{
    [Parameter]
    public ModalParametersModel ModalParameters { get; set; }

    [Inject]
    protected IStringLocalizer<AdminActivityResultEditModal> Localizer { get; set; }

    [Inject]
    protected IToastService Toast { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected ActivityResultModel ActivityResult { get; set; } = new();
    protected List<ActivityModel> Activities { get; set; } = new();
    protected List<TeamModel> Teams { get; set; } = new();
    protected List<PlayerResultModel> PlayerResults { get; set; } = new();
    protected string InstructionText { get; set; }
    protected int NumberOfPlayers { get; set; }

    protected override void OnInitialized()
    {
        Teams = DbContext.Teams.ToList();
        Activities = DbContext.Activities.ToList();

        if (ModalParameters.EditType != EditType.Add)
        {
            ActivityResult = DbContext.ActivityResults.FirstOrDefault(x => x.Id == ModalParameters.Id);
            var activity = Activities.FirstOrDefault(x => x.Id == ActivityResult.ActivityId);
            GetInstructionText(activity.ActivityType);
            NumberOfPlayers = activity.NumberOfPlayers;

            if (activity.NumberOfPlayers > 1)
            {
                PlayerResults = DbContext.PlayerResults.Where(x => x.ActivityResultId == ActivityResult.Id).ToList();
            }
        }
        else
        {
            var firstActivity = Activities.FirstOrDefault();
            if (firstActivity != null) ActivityResult.ActivityId = firstActivity.Id;

            GetInstructionText(firstActivity.ActivityType);
            NumberOfPlayers = firstActivity.NumberOfPlayers;

            var firstTeam = Teams.FirstOrDefault();
            if (firstTeam != null) ActivityResult.TeamId = firstTeam.Id;
        }
    }

    protected async Task Confirm()
    {
        try
        {
            if (ActivityResult.ActivityId > 0 && 
                ActivityResult.TeamId > 0 && 
                ActivityResult.Result >= 0)
            {
                if (ModalParameters.EditType == EditType.Add)
                {
                    await DbContext.ActivityResults.AddAsync(ActivityResult);
                    await DbContext.SaveChangesAsync();

                    for (int i = 0; i < PlayerResults.Count; i++)
                    {
                        PlayerResults[i].ActivityResultId = ActivityResult.Id;
                    }

                    DbContext.PlayerResults.UpdateRange(PlayerResults);
                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Update(ActivityResult);
                    DbContext.UpdateRange(PlayerResults);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Delete)
                {
                    if (NumberOfPlayers > 1)
                    {
                        DbContext.RemoveRange(PlayerResults);
                    }

                    DbContext.Remove(ActivityResult);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
            }
            else Toast.ShowWarning(Localizer["WarningToast"]);
        }
        catch
        {
            Toast.ShowError(Localizer["ErrorToast"]);
        }
        finally
        {
            await ModalService.Hide();
        }
    }

    protected void ResumValues(double value, int index)
    {
        PlayerResults[index].Result = value;

        ActivityResult.Result = 0;

        foreach (var playerResult in PlayerResults)
        {
            ActivityResult.Result += playerResult.Result;
        }
    }

    private void GetInstructionText(ActivityType activityType)
    {
        if (activityType == ActivityType.Time)
        {
            InstructionText = Localizer["TimeText"];
        }
        else if (activityType == ActivityType.Distance)
        {
            InstructionText = Localizer["DistanceText"];
        }
        else if (activityType == ActivityType.Points)
        {
            InstructionText = Localizer["PointsText"];
        }
    }

    protected Task OnActivitySelectedValueChanged(int value)
    {
        ActivityResult.ActivityId = value;
        var activity = Activities.FirstOrDefault(x => x.Id == value);
        GetInstructionText(activity.ActivityType);
        NumberOfPlayers = activity.NumberOfPlayers;

        if (NumberOfPlayers > 1)
        {
            PlayerResults = new();

            for (int i = 0; i < NumberOfPlayers; i++)
            {
                PlayerResults.Add(new PlayerResultModel());
            }
        }

        return Task.CompletedTask;
    }

    protected Task OnTeamSelectedValueChanged(int value)
    {
        ActivityResult.TeamId = value;

        return Task.CompletedTask;
    }
}
