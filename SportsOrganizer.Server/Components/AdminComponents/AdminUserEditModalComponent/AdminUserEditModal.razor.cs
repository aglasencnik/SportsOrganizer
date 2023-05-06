using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using System.Diagnostics;
using SportsOrganizer.Data.Enums;

namespace SportsOrganizer.Server.Components.AdminComponents.AdminUserEditModalComponent;

public class AdminUserEditModalBase : ComponentBase
{
    [Parameter]
    public ModalParametersModel ModalParameters { get; set; }

    [Inject]
    protected IStringLocalizer<AdminUserEditModal> Localizer { get; set; }

    [Inject]
    protected IToastService Toast { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected UserModel User { get; set; } = new();
    protected List<UserActivityModel> UserActivities { get; set; } = new();
    protected List<ActivityModel> Activities { get; set; } = new();
    protected IReadOnlyList<int> SelectedActivityIds { get; set; }
    protected TextRole Role { get; set; }
    protected bool ShowPassword { get; set; }

    protected override void OnInitialized()
    {
        if (ModalParameters.EditType != EditType.Add)
        {
            User = DbContext.Users.FirstOrDefault(x => x.Id == ModalParameters.Id);
            UserActivities = DbContext.UserActivities.Where(x => x.UserId == User.Id).ToList();
            SelectedActivityIds = UserActivities.Select(x => x.ActivityId).ToList();
        }

        User.UserType = UserType.User;
        Activities = DbContext.Activities.ToList();

        Role = TextRole.Password;
    }

    protected async Task Confirm()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(User.Username) &&
                !string.IsNullOrWhiteSpace(User.Password) &&
                User.UserType != UserType.Guest)
            {
                if (ModalParameters.EditType == EditType.Add)
                {
                    var existingUser = await DbContext.Users.FirstOrDefaultAsync(x => x.Username == User.Username);

                    if (existingUser == null)
                    {
                        await DbContext.Users.AddAsync(User);
                        await DbContext.SaveChangesAsync();

                        if (SelectedActivityIds != null && SelectedActivityIds.Count != 0)
                        {
                            foreach (var activityId in SelectedActivityIds)
                            {
                                UserActivities.Add(new UserActivityModel { ActivityId = activityId, UserId = User.Id });
                            }

                            await DbContext.UserActivities.AddRangeAsync(UserActivities);
                        }

                        await DbContext.SaveChangesAsync();

                        Toast.ShowSuccess(Localizer["SuccessToast"]);
                        await ModalService.Hide();
                    }
                    else Toast.ShowWarning(Localizer["DuplicateWarning"]);
                }
                else if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Users.Update(User);

                    var userActivityIds = UserActivities.Select(x => x.ActivityId).ToList();

                    if (SelectedActivityIds != null && SelectedActivityIds.Count != 0)
                    {
                        if (UserActivities != null && UserActivities.Count != 0)
                        {
                            var deletedUserActivityIds = userActivityIds.Except(SelectedActivityIds).ToList();
                            var deletedUserActivities = UserActivities.Where(x => deletedUserActivityIds.Contains(x.ActivityId)).ToList();
                            DbContext.UserActivities.RemoveRange(deletedUserActivities);

                            var newUserActivityIds = SelectedActivityIds.Except(userActivityIds).ToList();
                            var newUserActivities = new List<UserActivityModel>();

                            foreach (var activityId in newUserActivityIds)
                            {
                                newUserActivities.Add(new UserActivityModel { ActivityId = activityId, UserId = User.Id });
                            }

                            await DbContext.UserActivities.AddRangeAsync(newUserActivities);
                        }
                        else
                        {
                            var newUserActivities = new List<UserActivityModel>();

                            foreach (var activityId in SelectedActivityIds)
                            {
                                newUserActivities.Add(new UserActivityModel { ActivityId = activityId, UserId = User.Id });
                            }

                            await DbContext.UserActivities.AddRangeAsync(newUserActivities);
                        }
                    }
                    else if (UserActivities != null && UserActivities.Count != 0)
                    {
                        DbContext.UserActivities.RemoveRange(UserActivities);
                    }

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Delete)
                {
                    DbContext.Users.Remove(User);
                    if (UserActivities != null && UserActivities.Count != 0)
                        DbContext.UserActivities.RemoveRange(UserActivities);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);

                    await ModalService.Hide();
                }
            }
            else
            {
                if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Entry(User).State = EntityState.Detached;
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

    protected void TogglePasswordVisibility()
    {
        ShowPassword = !ShowPassword;
        if (ShowPassword) Role = TextRole.Text;
        else Role = TextRole.Password;
    }
}
