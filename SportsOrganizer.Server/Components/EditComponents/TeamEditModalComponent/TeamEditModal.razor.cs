using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Components.EditComponents.ActivityEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.EditComponents.TeamEditModalComponent;

public class TeamEditModalBase : ComponentBase
{
    [Parameter]
    public EditType EditType { get; set; }

    [Parameter]
    public int? TeamId { get; set; }

    [Inject]
    public IStringLocalizer<TeamEditModal> Localizer { get; set; }

    [Inject]
    public IToastService Toast { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public TeamModel Team { get; set; }

    protected override async Task OnInitializedAsync()
    {

    }
}
