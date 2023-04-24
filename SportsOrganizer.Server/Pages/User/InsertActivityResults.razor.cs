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
    public int TeamId { get; set; }
    public double Result { get; set; }

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
        Result = 0;
        TeamId = 0;

        int N = Activity.NumberOfPlayers;

        if (N > 1)
        {
            PlayerResults = new List<double>(new double[N]);
        }
        ErrorMessage = string.Empty;
    }

    public async Task InsertResult()
    {
        try
        {
            if (TeamId != 0 && Result >= 0)
            {
                int insertedActivityResultId;

                var existingActivityResult = DbContext.ActivityResults.FirstOrDefault(x => x.ActivityId == Activity.Id && x.TeamId == TeamId);

                if (existingActivityResult == null)
                {
                    var newActivityResult = new ActivityResultModel
                    {
                        ActivityId = Activity.Id,
                        TeamId = TeamId,
                        Result = Result
                    };

                    await DbContext.ActivityResults.AddAsync(newActivityResult);
                    await DbContext.SaveChangesAsync();

                    insertedActivityResultId = newActivityResult.Id;
                }
                else
                {
                    existingActivityResult.Result = Result;

                    if (Activity.NumberOfPlayers > 1)
                    {
                        var oldPlayerResults = DbContext.PlayerResults.Where(x => x.ActivityResultId == existingActivityResult.Id);
                        DbContext.PlayerResults.RemoveRange(oldPlayerResults);
                    }

                    await DbContext.SaveChangesAsync();

                    insertedActivityResultId = existingActivityResult.Id;
                }

                if (Activity.NumberOfPlayers > 1)
                {
                    List<PlayerResultModel> pResults = new();

                    foreach (var playerResult in PlayerResults)
                    {
                        pResults.Add(new PlayerResultModel 
                        { 
                            ActivityResultId = insertedActivityResultId, 
                            Result = playerResult 
                        });
                    }

                    await DbContext.PlayerResults.AddRangeAsync(pResults);
                    await DbContext.SaveChangesAsync();
                }

                Toast.ShowSuccess(Localizer["SuccessToast"]);
                ResetInputs();
            }
            else Toast.ShowWarning(Localizer["WarningToast"]);
        }
        catch
        {
            Toast.ShowError(Localizer["ErrorToast"]);
        }
    }

    public void ResumValues(double value, int index)
    {
        PlayerResults[index] = value;

        Result = PlayerResults.Sum();
    }
}
