using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.AdminComponents.AdminActivityEditModalComponent;

public class AdminActivityEditModalBase : ComponentBase
{
    [Parameter]
    public ModalParametersModel ModalParameters { get; set; }

    [Inject]
    protected IStringLocalizer<AdminActivityEditModal> Localizer { get; set; }

    [Inject]
    protected IToastService Toast { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public ActivityModel Activity { get; set; } = new();

    protected override void OnInitialized()
    {
        if (ModalParameters.EditType != EditType.Add)
        {
            Activity = DbContext.Activities.FirstOrDefault(x => x.Id == ModalParameters.Id);
        }
    }

    protected async Task Confirm()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Activity.Title) && 
                !string.IsNullOrWhiteSpace(Activity.Location) &&
                !string.IsNullOrWhiteSpace(Activity.Rules) &&
                !string.IsNullOrWhiteSpace(Activity.Props) &&
                Activity.NumberOfPlayers > 0)
            {
                if (ModalParameters.EditType == EditType.Add)
                {
                    var existingActivity = await DbContext.Activities.FirstOrDefaultAsync(x => x.ActivityNumber == Activity.ActivityNumber 
                                                                                            && x.Title == Activity.Title);

                    if (existingActivity == null)
                    {
                        await DbContext.Activities.AddAsync(Activity);

                        await DbContext.SaveChangesAsync();

                        Toast.ShowSuccess(Localizer["SuccessToast"]);
                        await ModalService.Hide();
                    }
                    else Toast.ShowWarning(Localizer["DuplicateWarning"]);
                }
                else if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Activities.Update(Activity);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Delete)
                {
                    var activityResults = DbContext.ActivityResults.Where(x => x.ActivityId == Activity.Id).ToList();
                    var userActivities = DbContext.UserActivities.Where(x => x.ActivityId == Activity.Id).ToList();

                    if (activityResults == null || activityResults.Count == 0
                        || userActivities == null || userActivities.Count == 0)
                    {
                        DbContext.Activities.Remove(Activity);

                        await DbContext.SaveChangesAsync();

                        Toast.ShowSuccess(Localizer["SuccessToast"]);
                    }
                    else Toast.ShowWarning(Localizer["CanNotDeleteToast"]);

                    await ModalService.Hide();
                }
            }
            else
            {
                if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Entry(Activity).State = EntityState.Detached;
                }
                Toast.ShowWarning(Localizer["WarningToast"]);
            }
        }
        catch
        {
            Toast.ShowError(Localizer["ErrorToast"]);
            await ModalService.Hide();
        }
    }
}
