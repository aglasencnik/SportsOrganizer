using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;
using SportsOrganizer.Data.Enums;

namespace SportsOrganizer.Server.Pages.User;

public class ActivityResultsOverviewSelectorBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<ActivityResultsOverviewSelector> Localizer { get; set; }

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

                    if (Activities.Count() == 1) NavigationManager.NavigateTo($"/User/Activity-Results-Overview/{Activities[0].Id}");
                }
            }
        }
    }
}
