using Blazored.Toast.Services;
using Blazorise;
using Blazorise.Bootstrap5;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Enums;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.Setup.BasicInfoSetupComponent;

public class BasicInfoSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }

    [Inject]
    public IStringLocalizer<BasicInfoSetup> Localizer { get; set; }

    [Inject]
    public ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    public bool isLoading { get; set; }
    public bool isDisabled { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Title { get; set; }
    public string CopyrightNotice { get; set; }
    public string HomePageHtml { get; set; }

    private IEnumerable<AppSettingsModel> liteDbResult;
    public Blazorise.FileEdit FileEdit { get; set; } = new();
    public IFileEntry faviconFile { get; set; }
    public string FaviconDataUrl { get; set; }

    protected override void OnInitialized()
    {
        liteDbResult = LiteDbService.GetAll();
        var faviconObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Favicon);

        FaviconDataUrl = (faviconObj != null) ? faviconObj.Value : "favicon.png";
    }

    public async Task OnFileChanged(FileChangedEventArgs e)
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

                FaviconDataUrl = $"data:image/png;base64,{Convert.ToBase64String(result.ToArray())}";
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
    }

    public Task ResetFileEdit()
    {
        liteDbResult = LiteDbService.GetAll();
        var faviconObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Favicon);

        FaviconDataUrl = (faviconObj != null) ? faviconObj.Value : "favicon.png";

        return FileEdit.Reset().AsTask();
    }

    public async Task OnButtonContinueClick()
    {
        try
        {
            isLoading = !isLoading;
            isDisabled = !isDisabled;

            if (!string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(Title) &&
                !string.IsNullOrWhiteSpace(CopyrightNotice))
            {
                var dbResult = DbContext.Users.FirstOrDefault(x => x.UserType == UserType.Admin);

                if (dbResult == null)
                {
                    await DbContext.Users.AddAsync(new UserModel
                    {
                        Username = Username,
                        Password = Password,
                        UserType = UserType.Admin
                    });
                }
                else
                {
                    dbResult.Username = Username;
                    dbResult.Password = Password;

                    DbContext.Update(dbResult);
                }
                await DbContext.SaveChangesAsync();

                var liteDbResult = LiteDbService.GetAll();
                var titleObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Title);
                var copyrightObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.CopyrightNotice);
                var homepageObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Homepage);
                var faviconObj = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Favicon);

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

                if (copyrightObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.CopyrightNotice,
                        Value = CopyrightNotice
                    });
                }
                else
                {
                    copyrightObj.Value = CopyrightNotice;

                    LiteDbService.Update(copyrightObj);
                }

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

                if (faviconObj == null)
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.Favicon,
                        Value = FaviconDataUrl
                    });
                }
                else
                {
                    faviconObj.Value = FaviconDataUrl;

                    LiteDbService.Update(faviconObj);
                }

                ToastService.ShowSuccess(Localizer["SuccessMessage"]);
                await OnSubmit.InvokeAsync(SetupStages.ThemeSetup);
            }
            else ToastService.ShowWarning(Localizer["WarningMessage"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorMessage"]);
        }
        finally
        {
            isLoading = !isLoading;
            isDisabled = !isDisabled;
        }
    }
}
