using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.UserComponents.UserActivityResultEditModalComponent;

public class UserActivityResultEditModalBase : ComponentBase
{
    [Parameter]
    public ModalParametersModel ModalParameters { get; set; }

    [Inject]
    public IStringLocalizer<UserActivityResultEditModal> Localizer { get; set; }

    [Inject]
    public IToastService Toast { get; set; }

    [Inject]
    public IModalService ModalService { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public ActivityResultModel ActivityResult { get; set; } = new();
    public ActivityModel Activity { get; set; } = new();
    public TeamModel Team { get; set; } = new();
    public List<PlayerResultModel> PlayerResults { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        ActivityResult = DbContext.ActivityResults.FirstOrDefault(x => x.Id == ModalParameters.Id);
        Activity = DbContext.Activities.FirstOrDefault(x => x.Id == ActivityResult.ActivityId);
        Team = DbContext.Teams.FirstOrDefault(x => x.Id == ActivityResult.TeamId);

        if (Activity.NumberOfPlayers > 1)
        {
            PlayerResults = DbContext.PlayerResults.Where(x => x.ActivityResultId == ActivityResult.Id).ToList();
        }
    }

    public async Task Confirm()
    {
        try
        {
            if (ActivityResult.Result >= 0)
            {
                if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Update(ActivityResult);
                    DbContext.UpdateRange(PlayerResults);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Delete)
                {
                    if (Activity.NumberOfPlayers > 1)
                    {
                        DbContext.Remove(PlayerResults);
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

    public void ResumValues(double value, int index)
    {
        PlayerResults[index].Result = value;

        foreach (var playerResult in PlayerResults)
        {
            ActivityResult.Result += playerResult.Result;
        }
    }
}
