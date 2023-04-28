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
    protected IStringLocalizer<UserActivityResultEditModal> Localizer { get; set; }

    [Inject]
    protected IToastService Toast { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected ActivityResultModel ActivityResult { get; set; } = new();
    protected ActivityModel Activity { get; set; } = new();
    protected TeamModel Team { get; set; } = new();
    protected List<PlayerResultModel> PlayerResults { get; set; } = new();

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

    protected async Task Confirm()
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

    protected void ResumValues(double value, int index)
    {
        PlayerResults[index].Result = value;

        foreach (var playerResult in PlayerResults)
        {
            ActivityResult.Result += playerResult.Result;
        }
    }
}
