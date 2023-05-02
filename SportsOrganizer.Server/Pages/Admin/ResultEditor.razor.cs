using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Components.AdminComponents.AdminActivityResultEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Admin;

public class ResultEditorBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<ResultEditor> Localizer { get; set; }

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

    protected List<ActivityResultModel> ActivityResults { get; set; }
    protected List<PlayerResultModel> PlayerResults { get; set; }
    protected List<ActivityModel> Activities { get; set; }
    protected List<TeamModel> Teams { get; set; }
    protected ThemeContrast ThemeContrast { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");

        ActivityResults = DbContext.ActivityResults.ToList();
        PlayerResults = DbContext.PlayerResults.ToList();
        Activities = DbContext.Activities.ToList();
        Teams = DbContext.Teams.ToList();

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

        ModalService.Show<AdminActivityResultEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenEditModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Edit,
            Id = id
        };

        ModalService.Show<AdminActivityResultEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenDeleteModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Delete,
            Id = id
        };

        ModalService.Show<AdminActivityResultEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
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
            DbContext.PlayerResults.RemoveRange(PlayerResults);
            DbContext.ActivityResults.RemoveRange(ActivityResults);
            await DbContext.SaveChangesAsync();
            ToastService.ShowSuccess(Localizer["SuccessToast"]);
            OnModalClosed();
        }
    }

    protected async Task Export(ExportFileType fileType)
    {
        if (fileType == ExportFileType.Xml)
        {
            (bool success, string message) data = await XmlSerializerService.SerializeActivitiesToXml(DbContext);

            if (data.success)
            {
                DateTime currentTime = DateTime.Now;
                await JSRuntime.InvokeVoidAsync("downloadFile", $"SportsOrganizer_activities_export_{currentTime.ToString("yyyy-MM-dd HH:mm:ss")}.xml", data.message);
            }
            else ToastService.ShowError(Localizer["ErrorToast"]);
        }
        else if (fileType == ExportFileType.Excel)
        {

        }
    }

    protected void Print()
    {

    }

    private void OnModalClosed()
    {
        ActivityResults = DbContext.ActivityResults.ToList();
        PlayerResults = DbContext.PlayerResults.ToList();
        Activities = DbContext.Activities.ToList();
        Teams = DbContext.Teams.ToList();
        StateHasChanged();
    }
}
