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

public class InsertActivityResultsSelectorBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<InsertActivityResultsSelector> Localizer { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public List<ActivityModel> Activities { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0) NavigationManager.NavigateTo("/");

        if (user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value == UserType.Admin.ToString())
            Activities = DbContext.Activities.ToList();
        else
        {
            int.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value, out int userId);

            if (userId != 0)
            {
                var availableActivityIds = DbContext.UserActivities.Where(x => x.UserId == userId).Select(x => x.ActivityId).ToList();

                if (availableActivityIds != null && availableActivityIds.Count() != 0)
                {
                    Activities = DbContext.Activities.Where(x => availableActivityIds.Contains(x.Id)).ToList();

                    if (Activities.Count() == 1) NavigationManager.NavigateTo($"/User/Insert-Activity-Results/{Activities[0].Id}");
                }
            }
        }
    }
}
