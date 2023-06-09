﻿using Microsoft.AspNetCore.Components;
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
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    protected ApplicationDbContextService DbContextService { get; set; }

    [Inject]
    protected CultureProviderService CultureProvider { get; set; }

    [Inject]
    protected ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    protected MemoryStorageUtility MemoryStorage { get; set; }

    protected override void OnInitialized()
    {
        if (MemoryStorage.Storage.Count == 0)
        {
            MemoryStorage.SetValuesFromLiteDb(LiteDbService.GetAll());
        }

        var language = MemoryStorage.GetValue(KeyValueType.LanguageShort);
        var culture = CultureInfo.GetCultureInfo((string)language);

        if (CultureProvider.GetCurrentCultureInfo().TwoLetterISOLanguageName != culture.Name)
        {
            var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var cultureEscaped = Uri.EscapeDataString(culture.Name);
            var uriEscaped = Uri.EscapeDataString(uri);
            NavigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
        }

        if (!DbContextService.IsDbContextSet())
        {
            var dbConnectionObj = MemoryStorage.GetValue(KeyValueType.Database);

            if (dbConnectionObj != null)
            {
                DatabaseConnectionModel dbConnectionModel = (DatabaseConnectionModel)dbConnectionObj;
                DbContextService.SetDbContext(dbConnectionModel.ConnectionString, dbConnectionModel.DatabaseProvider);
            }
        }
    }
}
