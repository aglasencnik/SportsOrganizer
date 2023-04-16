using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SportsOrganizer.Server.Components.Setup.LanguageSetupComponent;

public class LanguageSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }

    [Inject]
    public IConfiguration Configuration { get; set; }

    [Inject]
    public IStringLocalizer<LanguageSetup> Localizer { get; set; }

    [Inject]
    private ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject] 
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; }

    [Inject]
    public CultureProviderService CultureProvider { get; set; }

    public Dictionary<string, string> Languages { get; set; }

    public string SelectedLanguage { get; set; }

    protected override void OnInitialized()
    {
        Languages = Configuration.GetSection("Localization:Languages")
            .Get<Dictionary<string, string>>();

        var cookieProvider = new CookieRequestCultureProvider();
        HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieProvider.CookieName, out string cookieValue);

        if (!string.IsNullOrEmpty(cookieValue))
        {
            var cultureInfo = CookieRequestCultureProvider.ParseCookieValue(cookieValue);
            var culture = cultureInfo.Cultures[0];
            SelectedLanguage = !string.IsNullOrWhiteSpace(culture.ToString()) ? culture.ToString() : "en";
        }
        else
        {
            SelectedLanguage = "en";
        }
    }

    public void OnLanguageSelected(string value)
    {
        SelectedLanguage = value;
        var culture = CultureInfo.GetCultureInfo(value);

        if (CultureProvider.GetCurrentCultureInfo() != culture)
        {
            var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var cultureEscaped = Uri.EscapeDataString(culture.Name);
            var uriEscaped = Uri.EscapeDataString(uri);
            NavigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
        }
    }

    public async Task OnButtonContinueClick()
    {
        var liteDbResult = LiteDbService.GetAll();

        var languageSetting = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.LanguageShort);

        if (languageSetting != null)
        {
            languageSetting.Value = !string.IsNullOrWhiteSpace(SelectedLanguage) ? SelectedLanguage : "en";

            LiteDbService.Update(languageSetting);
        }
        else
        {
            LiteDbService.Insert(new AppSettingsModel
            {
                KeyValueType = KeyValueType.LanguageShort,
                Value = !string.IsNullOrWhiteSpace(SelectedLanguage) ? SelectedLanguage : "en"
            });
        }

        await OnSubmit.InvokeAsync(SetupStages.DatabaseSetup);
    }
}
