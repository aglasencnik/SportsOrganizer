using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.EditComponents.ActivityEditModalComponent;

public class ActivityEditModalBase : ComponentBase
{
    [Parameter]
    public EditType EditType { get; set; }

    [Parameter]
    public int? ActivityId { get; set; }

    [Inject]
    public IStringLocalizer<ActivityEditModal> Localizer { get; set; }

    [Inject]
    public IToastService Toast { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public ActivityModel Activity { get; set; }

    protected override async Task OnInitializedAsync()
    {
        
    }
}
