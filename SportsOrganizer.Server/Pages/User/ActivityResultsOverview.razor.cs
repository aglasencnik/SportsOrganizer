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
    public IStringLocalizer<ActivityResultsOverview> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IToastService Toast { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    [Inject]
    public IModalService ModalService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public List<TeamModel> Teams { get; set; } = new();
    public ActivityModel Activity { get; set; } = new();
    public List<ActivityResultModel> ActivityResults { get; set; } = new();
    public List<PlayerResultModel> PlayerResults { get; set; } = new();
    public ThemeContrast ThemeContrast { get; set; }

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

        if (themeObj != null) ThemeContrast = (ThemeContrast)themeObj;
        else ThemeContrast = ThemeContrast.Light;
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

    public void OpenEditModal(int id)
    {
        var modalParameters = new ModalParametersModel
        {
            EditType = EditType.Edit,
            Id = id
        };

        ModalService.Show<UserActivityResultEditModal>(x => x.Add(x => x.ModalParameters, modalParameters),
            new ModalInstanceOptions { Closed = new EventCallback(this, OnModalClosed), UseModalStructure = false });
    }

    public void OpenDeleteModal(int id)
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
