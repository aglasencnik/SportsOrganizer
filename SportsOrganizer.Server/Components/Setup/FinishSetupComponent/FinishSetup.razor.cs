using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.Setup.FinishSetupComponent;

public class FinishSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }

    [Inject]
    protected IStringLocalizer<FinishSetup> Localizer { get; set; }

    [Inject]
    protected ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected string Username { get; set; }
    protected string Password { get; set; }

    protected override void OnInitialized()
    {
        var dbResult = DbContext.Users.FirstOrDefault(x => x.UserType == Data.Enums.UserType.Admin);

        if (dbResult != null)
        {
            Username = dbResult.Username;
            int passLength = dbResult.Password.Length;

            for (int i = 0; i < passLength; i++)
            {
                Password += "*";
            }
        }
    }

    protected void OnButtonContinueClick()
    {
        var liteDbResult = LiteDbService.GetAll();
        var completedObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.SetupComplete);

        if (completedObj == null)
        {
            LiteDbService.Insert(new AppSettingsModel
            {
                KeyValueType = KeyValueType.SetupComplete,
                Value = true
            });
        }
        else
        {
            completedObj.Value = true;

            LiteDbService.Update(completedObj);
        }
        NavigationManager.NavigateTo("/account/login", true);
    }
}
