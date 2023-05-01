using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.AdminComponents.AdminTeamEditModalComponent;

public class AdminTeamEditModalBase : ComponentBase
{
    [Parameter]
    public ModalParametersModel ModalParameters { get; set; }

    [Inject]
    protected IStringLocalizer<AdminTeamEditModal> Localizer { get; set; }

    [Inject]
    protected IToastService Toast { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected TeamModel Team { get; set; } = new();

    protected override void OnInitialized()
    {
        if (ModalParameters.EditType != EditType.Add)
        {
            Team = DbContext.Teams.FirstOrDefault(x => x.Id == ModalParameters.Id);
        }
    }

    protected async Task Confirm()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Team.Name))
            {
                if (ModalParameters.EditType == EditType.Add)
                {
                    await DbContext.Teams.AddAsync(Team);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Teams.Update(Team);

                    await DbContext.SaveChangesAsync();

                    Toast.ShowSuccess(Localizer["SuccessToast"]);
                    await ModalService.Hide();
                }
                else if (ModalParameters.EditType == EditType.Delete)
                {
                    var teamActivityResults = DbContext.ActivityResults.Where(x => x.TeamId == Team.Id).ToList();

                    if (teamActivityResults == null || teamActivityResults.Count == 0)
                    {
                        DbContext.Teams.Remove(Team);

                        await DbContext.SaveChangesAsync();

                        Toast.ShowSuccess(Localizer["SuccessToast"]);
                    }
                    else Toast.ShowWarning(Localizer["CanNotDeleteToast"]);

                    await ModalService.Hide();
                }
            }
            else
            {
                if (ModalParameters.EditType == EditType.Edit)
                {
                    DbContext.Entry(Team).State = EntityState.Detached;
                }
                Toast.ShowWarning(Localizer["WarningToast"]);
            }
        }
        catch
        {
            Toast.ShowError(Localizer["ErrorToast"]);
            await ModalService.Hide();
        }
    }
}
