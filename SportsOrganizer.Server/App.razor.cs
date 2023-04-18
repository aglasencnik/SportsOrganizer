using Microsoft.AspNetCore.Components;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;
using System.Globalization;

namespace SportsOrganizer.Server;

public class AppBase : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    [Inject]
    public CultureProviderService CultureProvider { get; set; }

    [Inject]
    public ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    public MemoryStorageUtility MemoryStorageUtility { get; set; }

    protected override void OnInitialized()
    {
        if (MemoryStorageUtility.Storage.Count == 0)
        {
            MemoryStorageUtility.SetValuesFromLiteDb(LiteDbService.GetAll());
        }

        var language = MemoryStorageUtility.GetValue(KeyValueType.LanguageShort);
        var culture = CultureInfo.GetCultureInfo((string)language);

        if (CultureProvider.GetCurrentCultureInfo().Name != culture.Name)
        {
            var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var cultureEscaped = Uri.EscapeDataString(culture.Name);
            var uriEscaped = Uri.EscapeDataString(uri);
            NavigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
        }

        if (!DbContextService.IsDbContextSet())
        {
            var dbConnectionObj = MemoryStorageUtility.GetValue(KeyValueType.Database);

            if (dbConnectionObj != null)
            {
                DatabaseConnectionModel dbConnectionModel = (DatabaseConnectionModel)dbConnectionObj;
                DbContextService.SetDbContext(dbConnectionModel.ConnectionString, dbConnectionModel.DatabaseProvider);
            }
        }
    }
}
