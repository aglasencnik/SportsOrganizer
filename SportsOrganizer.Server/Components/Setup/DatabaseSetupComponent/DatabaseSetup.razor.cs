﻿using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportsOrganizer.MySqlMigrations;
using SportsOrganizer.PostgreSqlMigrations;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.SqlServerMigrations;

namespace SportsOrganizer.Server.Components.Setup.DatabaseSetupComponent;

public class DatabaseSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }

    [Inject]
    public IStringLocalizer<DatabaseSetup> Localizer { get; set; }

    [Inject]
    public ILiteDbService<AppSettingsModel> LiteDbService { get; set; }

    [Inject]
    public IToastService ToastService { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    public DatabaseProviderType SelectedDatabaseProvider { get; set; }

    public bool ShowForm { get; set; }

    public string Server { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }

    protected override void OnInitialized()
    {
        SelectedDatabaseProvider = DatabaseProviderType.SqlServer;

        ShowForm = true;

        Server = ".";
        Database = "sports_organizer_db";
    }

    public Task OnCheckedDatabaseProviderChanged(DatabaseProviderType provider)
    {
        SelectedDatabaseProvider = provider;

        if (SelectedDatabaseProvider == DatabaseProviderType.SqlServer)
        {
            Server = ".";
            Database = "sports_organizer_db";
        }
        else if (SelectedDatabaseProvider == DatabaseProviderType.MySQL)
        {
            Server = "localhost";
            Port = 3306;
            Database = "sports_organizer_db";
        }
        else if (SelectedDatabaseProvider == DatabaseProviderType.PostgreSQL)
        {
            Server = "localhost";
            Port = 5432;
            Database = "sports_organizer_db";
        }

        return Task.CompletedTask;
    }

    public async Task OnButtonContinueClick()
    {
        try
        {
            ShowForm = !ShowForm;
            await Task.Yield();
            StateHasChanged();

            if (SelectedDatabaseProvider == DatabaseProviderType.SqlServer &&
                !string.IsNullOrWhiteSpace(Server) &&
                !string.IsNullOrWhiteSpace(Database) &&
                !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password))
            {
                string connectionString = $"Server={Server};Database={Database};User Id={Username};Password={Password};Encrypt=False;";

                using (var db = new SqlServerDbContextFactory(connectionString))
                {
                    await db.Database.MigrateAsync();
                }

                DbContextService.SetDbContext(connectionString, SelectedDatabaseProvider);

                SaveConnectionStringToLiteDB(connectionString, SelectedDatabaseProvider);
                ToastService.ShowSuccess(Localizer["SuccessMessage"]);
                await OnSubmit.InvokeAsync(SetupStages.BasicInfoSetup);
            }
            else if (SelectedDatabaseProvider == DatabaseProviderType.MySQL &&
                !string.IsNullOrWhiteSpace(Server) &&
                !string.IsNullOrWhiteSpace(Database) &&
                !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password))
            {
                string connectionString = $"Server={Server};Port={Port};Database={Database};Uid={Username};Pwd={Password};";

                using (var db = new MySqlDbContextFactory(connectionString))
                {
                    await db.Database.MigrateAsync();
                }

                DbContextService.SetDbContext(connectionString, SelectedDatabaseProvider);

                SaveConnectionStringToLiteDB(connectionString, SelectedDatabaseProvider);
                ToastService.ShowSuccess(Localizer["SuccessMessage"]);
                await OnSubmit.InvokeAsync(SetupStages.BasicInfoSetup);
            }
            else if (SelectedDatabaseProvider == DatabaseProviderType.PostgreSQL &&
                !string.IsNullOrWhiteSpace(Server) &&
                !string.IsNullOrWhiteSpace(Database) &&
                !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password)
                )
            {
                string connectionString = $"Host={Server};Username={Username};Password={Password};Database={Database};";

                using (var db = new PostgreSqlDbContextFactory(connectionString))
                {
                    await db.Database.MigrateAsync();
                }

                DbContextService.SetDbContext(connectionString, SelectedDatabaseProvider);

                SaveConnectionStringToLiteDB(connectionString, SelectedDatabaseProvider);
                ToastService.ShowSuccess(Localizer["SuccessMessage"]);
                await OnSubmit.InvokeAsync(SetupStages.BasicInfoSetup);
            }
            else ToastService.ShowWarning(Localizer["WarningMessage"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorMessage"]);
        }
        finally
        {
            ShowForm = !ShowForm;
            await Task.Yield();
            StateHasChanged();
        }
    }

    private void SaveConnectionStringToLiteDB(string connectionString, DatabaseProviderType providerType)
    {
        var liteDbResult = LiteDbService.GetAll();
        var existingDatabase = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.Database);

        if (existingDatabase != null)
        {
            existingDatabase.Value = new DatabaseConnectionModel
            {
                DatabaseProvider = providerType,
                ConnectionString = connectionString
            };

            LiteDbService.Update(existingDatabase);
        }
        else
        {
            LiteDbService.Insert(new AppSettingsModel
            {
                KeyValueType = KeyValueType.Database,
                Value = new DatabaseConnectionModel
                {
                    DatabaseProvider = providerType,
                    ConnectionString = connectionString
                }
            });
        }
    }
}
