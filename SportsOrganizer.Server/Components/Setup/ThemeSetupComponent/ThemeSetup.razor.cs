using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;

namespace SportsOrganizer.Server.Components.Setup.ThemeSetupComponent;

public class ThemeSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }

    [Inject]
    protected ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    protected IStringLocalizer<ThemeSetup> Localizer { get; set; }

    [Inject]
    protected IToastService ToastService { get; set; }

    protected string BootswatchUrl { get; set; } = "https://bootswatch.com/";

    protected async Task SelectTheme(string theme)
    {
        var liteDbResult = LiteDbService.GetAll();
        var themeObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Theme);

        if (themeObj == null)
        {
            LiteDbService.Insert(new AppSettingsModel
            {
                KeyValueType = KeyValueType.Theme,
                Value = theme
            });
        }
        else
        {
            themeObj.Value = theme;

            LiteDbService.Update(themeObj);
        }

        ToastService.ShowSuccess(Localizer["SuccessMessage"]);
        await OnSubmit.InvokeAsync(SetupStages.FinishSetup);
    }
}
