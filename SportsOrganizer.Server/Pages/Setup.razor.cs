using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Pages;

public class SetupBase : ComponentBase
{
    [Inject]
    public IStringLocalizer<Setup> Localizer { get; set; }

    public SetupStages SetupStage;

    protected override async Task OnInitializedAsync()
    {
        SetupStage = SetupStages.LanguageSetup;
    }

    public void HandleSubmit(SetupStages stage)
    {
        SetupStage = stage;

        StateHasChanged();
    }
}
