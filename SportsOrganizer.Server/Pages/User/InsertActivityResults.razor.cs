using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.User;

public class InsertActivityResultsBase : ComponentBase
{
    [Parameter] 
    public int ActivityId { get; set; }

    [Inject]
    public IStringLocalizer<InsertActivityResults> Localizer { get; set; }

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

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public List<TeamModel> Teams { get; set; } = new();
    public ActivityModel Activity { get; set; } = new();
    public List<double> PlayerResults { get; set; } = new();
    public string ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0 || ActivityId <= 0 || !CheckIfAuthorized(user)) 
            NavigationManager.NavigateTo("/");

        Activity = DbContext.Activities.FirstOrDefault(x => x.Id == ActivityId);

        if (Activity == null) NavigationManager.NavigateTo("/");

        Teams = DbContext.Teams.ToList();
        ResetInputs();
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

    private void ResetInputs()
    {
        int N = Activity.NumberOfPlayers;

        if (N > 1)
        {
            PlayerResults = new List<double>(new double[N]);
        }
        ErrorMessage = string.Empty;
    }

    public async Task InsertResult()
    {

    }
}
