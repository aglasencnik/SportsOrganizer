using Blazored.Toast.Services;
using Blazorise;
using LiteDB;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Components.UserComponents.UserActivityResultEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.User;

public class ActivityResultsOverviewBase : ComponentBase
{
    [Parameter]
    public int ActivityId { get; set; }

    [Inject]
    protected IStringLocalizer<ActivityResultsOverview> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected IToastService Toast { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected List<TeamModel> Teams { get; set; } = new();
    protected ActivityModel Activity { get; set; } = new();
    protected List<ActivityResultModel> ActivityResults { get; set; } = new();
    protected List<PlayerResultModel> PlayerResults { get; set; } = new();
    protected ThemeContrast ThemeContrast { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || ActivityId <= 0 || !CheckIfAuthorized(user))
            NavigationManager.NavigateTo("/");

        Activity = DbContext.Activities.FirstOrDefault(x => x.Id == ActivityId);

        if (Activity == null) NavigationManager.NavigateTo("/");

        ActivityResults = DbContext.ActivityResults.Where(x => x.ActivityId == ActivityId).ToList();
        Teams = DbContext.Teams.ToList();

        var themeObj = MemoryStorage.GetValue(KeyValueType.DataGridThemeContrast);
        ThemeContrast = (themeObj == null
            || !Enum.TryParse(themeObj.ToString(), out ThemeContrast result))
            ? ThemeContrast.Light
            : result;
    }

    private bool CheckIfAuthorized(ClaimsPrincipal user)
    {
        var userType = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

        if (userType == UserType.Admin.ToString()) return true;

        int.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value, out int userId);

        if (userId > 0)
        {
            var authorizedActivity = DbContext.UserActivities.FirstOrDefault(x => x.UserId == userId && x.ActivityId == ActivityId);

            if (authorizedActivity == null) return false;
            else return true;
        }
        else return false;
    }

    protected void OpenEditModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Edit,
            Id = id
        };

        ModalService.Show<UserActivityResultEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    protected void OpenDeleteModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Delete,
            Id = id
        };

        ModalService.Show<UserActivityResultEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    private void OnModalClosed()
    {
        ActivityResults = DbContext.ActivityResults.Where(x => x.ActivityId == ActivityId).ToList();
        StateHasChanged();
    }
}
