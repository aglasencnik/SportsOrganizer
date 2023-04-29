using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Components.AdminComponents.AdminTeamEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Admin;

public class TeamManagementBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<TeamManagement> Localizer { get; set; }

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
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected List<TeamModel> Teams { get; set; } = new();
    protected ThemeContrast ThemeContrast { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");

        Teams = DbContext.Teams.ToList();

        var themeObj = MemoryStorage.GetValue(KeyValueType.DataGridThemeContrast);

        if (themeObj != null) ThemeContrast = (ThemeContrast)themeObj;
        else ThemeContrast = ThemeContrast.Light;
    }

    protected void OpenAddModal()
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Add
        };

        ModalService.Show<AdminTeamEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenEditModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Edit,
            Id = id
        };

        ModalService.Show<AdminTeamEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenDeleteModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Delete,
            Id = id
        };

        ModalService.Show<AdminTeamEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected async Task DeleteAll()
    {
        if (await MessageService.Confirm(Localizer["ConfModalContent"], Localizer["ConfModalHeader"]))
        {
            var teamIds = Teams.Select(t => t.Id);
            var result = DbContext.ActivityResults.Where(x => teamIds.Contains(x.TeamId)).ToList();

            if (result == null || result.Count() == 0)
            {
                DbContext.Teams.RemoveRange(Teams);
                await DbContext.SaveChangesAsync();
                ToastService.ShowSuccess(Localizer["SuccessToast"]);
                OnModalClosed();
            }
            else ToastService.ShowWarning(Localizer["WarningToast"]);
        }
    }

    protected void Export(ExportFileType fileType)
    {
        
    }

    protected void Print()
    {

    }

    private void OnModalClosed()
    {
        Teams = DbContext.Teams.ToList();
        StateHasChanged();
    }
}
