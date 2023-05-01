using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Utils;
using System.Globalization;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Admin;

public class SettingsBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<Settings> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [Inject]
    protected ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected IToastService ToastService { get; set; }

    [Inject]
    protected IConfiguration Configuration { get; set; }

    protected FileEdit FileEdit { get; set; } = new();
    protected Dictionary<string, string> Languages { get; set; }

    protected string Language { get; set; }
    protected string Title { get; set; }
    protected string CopyrightNotice { get; set; }
    protected string Favicon { get; set; }
    protected ScoringType ScoringType { get; set; }
    protected string HeaderColor { get; set; }
    protected string FooterColor { get; set; }
    protected string FinalScoresTableColor { get; set; }
    protected string SingleActivityTableColor { get; set; }
    protected ThemeContrast DataGridThemeContrast { get; set; }
    protected ThemeContrast AdminSidebarThemeContrast { get; set; }
    protected ThemeContrast AdminHeaderThemeContrast { get; set; }
    protected string AdminHeaderBackground { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");

        Languages = Configuration.GetSection("Localization:Languages").Get<Dictionary<string, string>>();

        var languageObj = MemoryStorage.GetValue(KeyValueType.LanguageShort);
        var titleObj = MemoryStorage.GetValue(KeyValueType.Title);
        var copyrightNoticeObj = MemoryStorage.GetValue(KeyValueType.CopyrightNotice);
        var faviconObj = MemoryStorage.GetValue(KeyValueType.Favicon);
        var scoringTypeObj = MemoryStorage.GetValue(KeyValueType.ScoringType);
        var headerColorObj = MemoryStorage.GetValue(KeyValueType.HeaderColor);
        var footerColorObj = MemoryStorage.GetValue(KeyValueType.FooterColor);
        var finalScoresTableColorObj = MemoryStorage.GetValue(KeyValueType.FinalScoresTableColor);
        var singleActivityTableColorObj = MemoryStorage.GetValue(KeyValueType.SingleActivityTableColor);
        var dataGridThemeContrastObj = MemoryStorage.GetValue(KeyValueType.DataGridThemeContrast);
        var adminSidebarThemeContrastObj = MemoryStorage.GetValue(KeyValueType.AdminLayoutBarThemeContrast);
        var adminHeaderThemeContrastObj = MemoryStorage.GetValue(KeyValueType.AdminLayoutHeaderThemeContrast);
        var adminHeaderBackgroundObj = MemoryStorage.GetValue(KeyValueType.AdminLayoutHeaderBackground);

        Language = (languageObj != null) ? (string)languageObj : "en";
        Title = (titleObj != null) ? (string)titleObj : "SportsOrganizer";
        CopyrightNotice = (copyrightNoticeObj != null) ? (string)copyrightNoticeObj : "SportsOrganizer";
        Favicon = (faviconObj != null) ? (string)faviconObj : "favicon.png";
        ScoringType = (scoringTypeObj == null
            || !Enum.TryParse(scoringTypeObj.ToString(), out ScoringType scoringType))
            ? ScoringType.Ascending
            : scoringType;
        HeaderColor = (headerColorObj != null) ? (string)headerColorObj : NavbarColor.Primary;
        FooterColor = (footerColorObj != null) ? (string)footerColorObj : NavbarColor.Dark;
        FinalScoresTableColor = (finalScoresTableColorObj != null) ? (string)finalScoresTableColorObj : TableColor.Default;
        SingleActivityTableColor = (singleActivityTableColorObj != null) ? (string)singleActivityTableColorObj : TableColor.Default;
        DataGridThemeContrast = (dataGridThemeContrastObj == null
            || !Enum.TryParse(dataGridThemeContrastObj.ToString(), out ThemeContrast dataGridThemeContrast))
            ? ThemeContrast.Light
            : dataGridThemeContrast;
        AdminSidebarThemeContrast = (adminSidebarThemeContrastObj == null
            || !Enum.TryParse(adminSidebarThemeContrastObj.ToString(), out ThemeContrast adminSidebarThemeContrast))
            ? ThemeContrast.Light
            : adminSidebarThemeContrast;
        AdminHeaderThemeContrast = (adminHeaderThemeContrastObj == null
            || !Enum.TryParse(adminHeaderThemeContrastObj.ToString(), out ThemeContrast adminHeaderThemeContrast))
            ? ThemeContrast.Light
            : adminHeaderThemeContrast;
        AdminHeaderBackground = (adminHeaderBackgroundObj != null) ? (string)adminHeaderBackgroundObj : Background.Light.Name;
    }

    protected void UpdateBasicInfo()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Language) &&
                !string.IsNullOrWhiteSpace(Title) &&
                !string.IsNullOrWhiteSpace(CopyrightNotice) &&
                !string.IsNullOrWhiteSpace(Favicon))
            {
                var liteDbResult = LiteDbService.GetAll();
                var languageObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.LanguageShort);

                if (languageObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.LanguageShort,
                        Value = Language
                    });
                }
                else
                {
                    languageObj.Value = Language;

                    LiteDbService.Update(languageObj);
                }

                var titleObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Title);

                if (titleObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.Title,
                        Value = Title
                    });
                }
                else
                {
                    titleObj.Value = Title;

                    LiteDbService.Update(titleObj);
                }

                var copyrightNoticeObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.CopyrightNotice);

                if (copyrightNoticeObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.CopyrightNotice,
                        Value = CopyrightNotice
                    });
                }
                else
                {
                    copyrightNoticeObj.Value = CopyrightNotice;

                    LiteDbService.Update(copyrightNoticeObj);
                }

                var faviconObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Favicon);

                if (faviconObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.Favicon,
                        Value = Favicon
                    });
                }
                else
                {
                    faviconObj.Value = Favicon;

                    LiteDbService.Update(faviconObj);
                }

                var scoringTypeObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.ScoringType);

                if (scoringTypeObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.ScoringType,
                        Value = ScoringType
                    });
                }
                else
                {
                    scoringTypeObj.Value = ScoringType;

                    LiteDbService.Update(scoringTypeObj);
                }

                MemoryStorage.ClearAll();
                MemoryStorage.SetValuesFromLiteDb(LiteDbService.GetAll());

                var culture = CultureInfo.GetCultureInfo(Language);
                var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                var cultureEscaped = Uri.EscapeDataString(culture.Name);
                var uriEscaped = Uri.EscapeDataString(uri);
                NavigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
            }
            else ToastService.ShowWarning(Localizer["WarningToast"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorToast"]);
        }
    }

    protected void UpdateThemeCostumization()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(HeaderColor) &&
                !string.IsNullOrWhiteSpace(FooterColor) &&
                FinalScoresTableColor != null &&
                SingleActivityTableColor != null &&
                DataGridThemeContrast != ThemeContrast.None &&
                AdminSidebarThemeContrast != ThemeContrast.None &&
                AdminHeaderThemeContrast != ThemeContrast.None &&
                AdminHeaderBackground != null)
            {
                var liteDbResult = LiteDbService.GetAll();
                var headerColorObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.HeaderColor);

                if (headerColorObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.HeaderColor,
                        Value = HeaderColor
                    });
                }
                else
                {
                    headerColorObj.Value = HeaderColor;

                    LiteDbService.Update(headerColorObj);
                }

                var footerColorObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.FooterColor);

                if (footerColorObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.FooterColor,
                        Value = FooterColor
                    });
                }
                else
                {
                    footerColorObj.Value = FooterColor;

                    LiteDbService.Update(footerColorObj);
                }

                var finalScoresTableColorObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.FinalScoresTableColor);

                if (finalScoresTableColorObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.FinalScoresTableColor,
                        Value = FinalScoresTableColor
                    });
                }
                else
                {
                    finalScoresTableColorObj.Value = FinalScoresTableColor;

                    LiteDbService.Update(finalScoresTableColorObj);
                }

                var singleActivityTableColorObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.SingleActivityTableColor);

                if (singleActivityTableColorObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.SingleActivityTableColor,
                        Value = SingleActivityTableColor
                    });
                }
                else
                {
                    singleActivityTableColorObj.Value = SingleActivityTableColor;

                    LiteDbService.Update(singleActivityTableColorObj);
                }

                var dataGridThemeContrastObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.DataGridThemeContrast);

                if (dataGridThemeContrastObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.DataGridThemeContrast,
                        Value = DataGridThemeContrast
                    });
                }
                else
                {
                    dataGridThemeContrastObj.Value = DataGridThemeContrast;

                    LiteDbService.Update(dataGridThemeContrastObj);
                }

                var adminSidebarThemeContrastObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.AdminLayoutBarThemeContrast);

                if (adminSidebarThemeContrastObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.AdminLayoutBarThemeContrast,
                        Value = AdminSidebarThemeContrast
                    });
                }
                else
                {
                    adminSidebarThemeContrastObj.Value = AdminSidebarThemeContrast;

                    LiteDbService.Update(adminSidebarThemeContrastObj);
                }

                var adminHeaderThemeContrastObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.AdminLayoutHeaderThemeContrast);

                if (adminHeaderThemeContrastObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.AdminLayoutHeaderThemeContrast,
                        Value = AdminHeaderThemeContrast
                    });
                }
                else
                {
                    adminHeaderThemeContrastObj.Value = AdminHeaderThemeContrast;

                    LiteDbService.Update(adminHeaderThemeContrastObj);
                }

                var adminHeaderBackgroundObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.AdminLayoutHeaderBackground);

                if (adminHeaderBackgroundObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.AdminLayoutHeaderBackground,
                        Value = AdminHeaderBackground
                    });
                }
                else
                {
                    adminHeaderBackgroundObj.Value = AdminHeaderBackground;

                    LiteDbService.Update(adminHeaderBackgroundObj);
                }

                MemoryStorage.ClearAll();
                MemoryStorage.SetValuesFromLiteDb(LiteDbService.GetAll());

                var culture = CultureInfo.GetCultureInfo(Language);
                var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                var cultureEscaped = Uri.EscapeDataString(culture.Name);
                var uriEscaped = Uri.EscapeDataString(uri);
                NavigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
            }
            else ToastService.ShowWarning(Localizer["WarningToast"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorToast"]);
        }
    }

    protected async Task OnFileChanged(FileChangedEventArgs e)
    {
        try
        {
            var file = e.Files.FirstOrDefault();
            if (file == null)
            {
                return;
            }

            using (MemoryStream result = new MemoryStream())
            {
                await file.OpenReadStream(long.MaxValue).CopyToAsync(result);

                Favicon = $"data:image/png;base64,{Convert.ToBase64String(result.ToArray())}";
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
    }

    protected Task ResetFileEdit()
    {
        var faviconObj = MemoryStorage.GetValue(KeyValueType.Favicon);

        Favicon = (faviconObj != null) ? (string)faviconObj : "favicon.png";

        return FileEdit.Reset().AsTask();
    }
}
