using Blazored.Toast.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;

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

    public DatabaseProviderType SelectedDatabaseProvider { get; set; }

    public string Server { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool Pooling { get; set; }
    public int MinPoolSize { get; set; }
    public int MaxPoolSize { get; set; }
    public int ConnectionLifetime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SelectedDatabaseProvider = DatabaseProviderType.SqlServer;

        Password = string.Empty;
        MinPoolSize = 0;
        MaxPoolSize = 100;
        ConnectionLifetime = 0;
    }

    public Task OnCheckedDatabaseProviderChanged(DatabaseProviderType provider)
    {
        SelectedDatabaseProvider = provider;

        return Task.CompletedTask;
    }

    public async Task OnButtonContinueClick()
    {
        try
        {
            var liteDbResult = LiteDbService.GetAll();
            var existingConnectionString = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.ConnectionString);
            var existingDatabaseProvider = liteDbResult.FirstOrDefault(x => x.KeyValueType == KeyValueType.DatabaseProvider);

            if (SelectedDatabaseProvider == DatabaseProviderType.SqlServer &&
                !string.IsNullOrWhiteSpace(Server) &&
                !string.IsNullOrWhiteSpace(Database) &&
                !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password))
            {
                string connectionString = $"Server={Server};Database={Database};User Id={Username};Password={Password};";



                if (existingConnectionString != null && existingDatabaseProvider != null)
                {
                    existingConnectionString.Value = connectionString;
                    existingDatabaseProvider.Value = ((int)SelectedDatabaseProvider).ToString();

                    LiteDbService.Update(existingConnectionString);
                    LiteDbService.Update(existingDatabaseProvider);
                }
                else
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.ConnectionString,
                        Value = connectionString
                    });

                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.DatabaseProvider,
                        Value = connectionString
                    });
                }

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



                if (existingConnectionString != null && existingDatabaseProvider != null)
                {
                    existingConnectionString.Value = connectionString;
                    existingDatabaseProvider.Value = ((int)SelectedDatabaseProvider).ToString();

                    LiteDbService.Update(existingConnectionString);
                    LiteDbService.Update(existingDatabaseProvider);
                }
                else
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.ConnectionString,
                        Value = connectionString
                    });

                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.DatabaseProvider,
                        Value = connectionString
                    });
                }

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
                string connectionString = $"User ID={Username};Password={Password};Host={Server};Port={Port};Database={Database};Pooling={Pooling};Min Pool Size={MinPoolSize};Max Pool Size={MaxPoolSize};Connection Lifetime={ConnectionLifetime};";



                if (existingConnectionString != null && existingDatabaseProvider != null)
                {
                    existingConnectionString.Value = connectionString;
                    existingDatabaseProvider.Value = ((int)SelectedDatabaseProvider).ToString();

                    LiteDbService.Update(existingConnectionString);
                    LiteDbService.Update(existingDatabaseProvider);
                }
                else
                {
                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.ConnectionString,
                        Value = connectionString
                    });

                    LiteDbService.Insert(new AppSettingsModel
                    {
                        KeyValueType = KeyValueType.DatabaseProvider,
                        Value = connectionString
                    });
                }

                ToastService.ShowSuccess(Localizer["SuccessMessage"]);
                await OnSubmit.InvokeAsync(SetupStages.BasicInfoSetup);
            }
            else ToastService.ShowWarning(Localizer["WarningMessage"]);
        }
        catch
        {
            ToastService.ShowError(Localizer["ErrorMessage"]);
        }
    }
}
