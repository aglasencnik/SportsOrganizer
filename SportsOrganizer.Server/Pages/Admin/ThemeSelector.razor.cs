using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SportsOrganizer.Server.Pages.Admin;

public class ThemeSelectorBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<ThemeSelector> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    protected IToastService ToastService { get; set; }

    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    protected string BootswatchUrl { get; set; } = "https://bootswatch.com/";
    protected string OldTheme { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");

        var oldThemeObj = MemoryStorage.GetValue(KeyValueType.Theme);
        OldTheme = (oldThemeObj != null) ? (string)oldThemeObj : string.Empty;
    }

    protected async Task SelectTheme(string theme)
    {
        try
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

            MemoryStorage.SetValue(KeyValueType.Theme, theme);
            await JSRuntime.InvokeVoidAsync("changeTheme", theme);
            OldTheme = theme;
            StateHasChanged();
            ToastService.ShowSuccess(Localizer["SuccessToast"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorToast"]);
        }
    }
}
