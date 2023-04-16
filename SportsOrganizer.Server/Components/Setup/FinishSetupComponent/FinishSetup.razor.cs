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
    public IStringLocalizer<FinishSetup> Localizer { get; set; }

    [Inject]
    public ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public string Username { get; set; }
    public string Password { get; set; }

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

    public void OnButtonContinueClick()
    {
        var liteDbResult = LiteDbService.GetAll();
        var completedObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.SetupComplete);

        if (completedObj == null)
        {
            LiteDbService.Insert(new AppSettingsModel
            {
                KeyValueType = KeyValueType.SetupComplete,
                BoolValue = true
            });
        }
        else
        {
            completedObj.BoolValue = true;

            LiteDbService.Update(completedObj);
        }
        NavigationManager.NavigateTo("/account/login", true);
    }
}
