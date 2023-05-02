using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Org.BouncyCastle.Utilities;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Components.AdminComponents.AdminUserEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Admin;

public class UserManagementBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<UserManagement> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected IToastService ToastService { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected IMessageService MessageService { get; set; }

    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public List<UserModel> Users { get; set; }
    protected List<UserActivityModel> UserActivities { get; set; }
    protected List<ActivityModel> Activities { get; set; }
    protected ThemeContrast ThemeContrast { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");

        Users = DbContext.Users.ToList();
        UserActivities = DbContext.UserActivities.ToList();
        var activityIds = UserActivities.Select(x => x.ActivityId).ToList();
        Activities = DbContext.Activities.Where(x => activityIds.Contains(x.Id)).ToList();

        var themeObj = MemoryStorage.GetValue(KeyValueType.DataGridThemeContrast);
        ThemeContrast = (themeObj == null
            || !Enum.TryParse(themeObj.ToString(), out ThemeContrast result))
            ? ThemeContrast.Light
            : result;
    }

    protected void OpenAddModal()
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Add
        };

        ModalService.Show<AdminUserEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenEditModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Edit,
            Id = id
        };

        ModalService.Show<AdminUserEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenDeleteModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Delete,
            Id = id
        };

        ModalService.Show<AdminUserEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected async Task DeleteAll()
    {
        if (await MessageService.Confirm(Localizer["ConfModalContent"], Localizer["ConfModalHeader"], opt => 
        { 
            opt.ConfirmButtonText = Localizer["Confirm"]; 
            opt.CancelButtonText = Localizer["Cancel"]; 
        }))
        {
            var firstAdmin = Users.FirstOrDefault(x => x.UserType == UserType.Admin);

            var usersToDelete = Users.Where(x => x.Id != firstAdmin.Id).ToList();
            var userIds = usersToDelete.Select(x => x.Id);
            var userActivitiesToDelete = UserActivities.Where(x => userIds.Contains(x.UserId)).ToList();

            DbContext.UserActivities.RemoveRange(userActivitiesToDelete);
            DbContext.Users.RemoveRange(usersToDelete);
            await DbContext.SaveChangesAsync();
            ToastService.ShowSuccess(Localizer["SuccessToast"]);
            OnModalClosed();
        }
    }

    protected async Task Export(ExportFileType fileType)
    {
        if (fileType == ExportFileType.Xml)
        {
            (bool success, string message) data = await XmlSerializerService.SerializeUsersToXml(DbContext);

            if (data.success)
            {
                DateTime currentTime = DateTime.Now;
                await JSRuntime.InvokeVoidAsync("saveAsXml", $"SportsOrganizer_users_export_{currentTime.ToString("yyyy-MM-dd HH:mm:ss")}.xml", data.message);
            }
            else ToastService.ShowError(Localizer["ErrorToast"]);
        }
        else if (fileType == ExportFileType.Excel)
        {
            (bool success, byte[] message) data = await ExcelSerializerService.SerializeUsersToExcel(DbContext);

            if (data.success)
            {
                DateTime currentTime = DateTime.Now;
                var base64 = Convert.ToBase64String(data.message);
                await JSRuntime.InvokeVoidAsync("saveAsXlsx", $"SportsOrganizer_users_export_{currentTime.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx", base64);
            }
            else ToastService.ShowError(Localizer["ErrorToast"]);
        }
    }

    protected void Print()
    {

    }

    private void OnModalClosed()
    {
        Users = DbContext.Users.ToList();
        UserActivities = DbContext.UserActivities.ToList();
        var activityIds = UserActivities.Select(x => x.ActivityId).ToList();
        Activities = DbContext.Activities.Where(x => activityIds.Contains(x.Id)).ToList();
        StateHasChanged();
    }
}
