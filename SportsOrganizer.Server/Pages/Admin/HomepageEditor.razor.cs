using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Utils;
using System.Security.Claims;

namespace SportsOrganizer.Server.Pages.Admin;

public class HomepageEditorBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<HomepageEditor> Localizer { get; set; }

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

    protected string HomePageHtml { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");

        var homepageObj = MemoryStorage.GetValue(KeyValueType.Homepage);

        HomePageHtml = (homepageObj != null) ? (string)homepageObj : string.Empty;
    }

    protected void SaveChanges()
    {
        try
        {
            var homepageObj = LiteDbService.GetAll().FirstOrDefault(x => x.KeyValueType == KeyValueType.Homepage);

            if (homepageObj == null)
            {
                LiteDbService.Insert(new AppSettingsModel
                {
                    KeyValueType = KeyValueType.Homepage,
                    Value = HomePageHtml
                });
            }
            else
            {
                homepageObj.Value = HomePageHtml;

                LiteDbService.Update(homepageObj);
            }

            MemoryStorage.SetValue(KeyValueType.Homepage, HomePageHtml);
            ToastService.ShowSuccess(Localizer["SucccessToast"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorToast"]);
        }
    }
}
