using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace SportsOrganizer.Server.Pages.Admin;

public class BackupAndRestoreBase : ComponentBase
{
    [Inject]
    protected IStringLocalizer<BackupAndRestore> Localizer { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected IToastService ToastService { get; set; }

    [Inject]
    protected IModalService ModalService { get; set; }

    [Inject]
    protected IMessageService MessageService { get; set; }

    [Inject]
    protected IJSRuntime JSRuntime { get; set; }

    [Inject]
    protected ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected FileEdit FileEdit { get; set; } = new();
    protected string XmlContent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var user = authState.User;

        if (user.Identities.Count() == 0
            || user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value != UserType.Admin.ToString())
            NavigationManager.NavigateTo("/");
    }

    protected async Task Backup()
    {
        (bool success, string message) data = await XmlBackupService.GenerateBackup(DbContext, LiteDbService);

        if (data.success)
        {
            DateTime currentTime = DateTime.Now;
            await JSRuntime.InvokeVoidAsync("saveAsXml", $"SportsOrganizer_backup_{currentTime.ToString("yyyy-MM-dd HH:mm:ss")}.xml", data.message);
        }
        else ToastService.ShowError(Localizer["ErrorToast"]);
    }

    protected async Task Restore()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(XmlContent))
            {
                if (await MessageService.Confirm(Localizer["RestorationQuestion"], Localizer["ConfirmRestoration"], opt =>
                {
                    opt.ConfirmButtonText = Localizer["Confirm"];
                    opt.CancelButtonText = Localizer["Cancel"];
                }))
                {
                    if (await XmlBackupService.RestoreApplication(DbContext, LiteDbService, XmlContent))
                    {
                        ToastService.ShowSuccess(Localizer["SuccessToast"]);
                    }
                    else
                    {
                        ToastService.ShowError(Localizer["ErrorToast"]);
                    }

                    MemoryStorage.ClearAll();
                    MemoryStorage.SetValuesFromLiteDb(LiteDbService.GetAll());

                    var culture = CultureInfo.GetCultureInfo((string)MemoryStorage.GetValue(Enums.KeyValueType.LanguageShort));
                    var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                    var cultureEscaped = Uri.EscapeDataString(culture.Name);
                    var uriEscaped = Uri.EscapeDataString(uri);
                    NavigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
                }
            }
            else ToastService.ShowWarning(Localizer["CheckFile"]);
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
                XmlContent = Encoding.UTF8.GetString(result.ToArray());
            }
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorToast"]);
        }
    }
}
