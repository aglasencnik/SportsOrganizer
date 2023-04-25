using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Components.EditComponents.ActivityEditModalComponent;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.EditComponents.UserEditModalComponent;

public class UserEditModalBase : ComponentBase
{
    [Parameter]
    public EditType EditType { get; set; }

    [Parameter]
    public int? UserId { get; set; }

    [Inject]
    public IStringLocalizer<UserEditModal> Localizer { get; set; }

    [Inject]
    public IToastService Toast { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public UserModel User { get; set; }

    protected override async Task OnInitializedAsync()
    {

    }
}
