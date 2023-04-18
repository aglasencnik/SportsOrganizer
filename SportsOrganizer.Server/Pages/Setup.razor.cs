using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;

namespace SportsOrganizer.Server.Pages;

public class SetupBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<Setup> Localizer { get; set; }

    public SetupStages SetupStage;

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    protected override void OnInitialized()
    {
        SetupStage = SetupStages.LanguageSetup;

        var dbResult = LiteDbService.GetAll().FirstOrDefault(x => x.KeyValueType == KeyValueType.SetupComplete);

        if (dbResult != null && (bool)dbResult.Value == true) NavigationManager.NavigateTo("/");
    }

    public void HandleSubmit(SetupStages stage)
    {
        SetupStage = stage;

        StateHasChanged();
    }
}
