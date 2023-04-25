using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Components.EditComponents.ActivityEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.EditComponents.ActivityResultEditModalComponent;

public class ActivityResultEditModalBase : ComponentBase
{
    [Parameter]
    public EditType EditType { get; set; }

    [Parameter]
    public int? ActivityResultId { get; set; }

    [Inject]
    public IStringLocalizer<ActivityResultEditModal> Localizer { get; set; }

    [Inject]
    public IToastService Toast { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public ActivityResultModel ActivityResult { get; set; }

    protected override async Task OnInitializedAsync()
    {

    }
}
