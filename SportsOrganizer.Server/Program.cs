using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddControllers();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
})
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddScoped(typeof(ILiteDbService<>), typeof(LiteDbService<>));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var languages = configuration.GetSection("Localization:Languages").Get<Dictionary<string, string>>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("en");
    options.AddSupportedCultures(languages.Keys.ToArray());
    options.AddSupportedUICultures(languages.Keys.ToArray());
    options.RequestCultureProviders = new List<IRequestCultureProvider>()
    {
        new CookieRequestCultureProvider()
    };
});

builder.Services.AddBlazoredToast();

var liteDbService = new LiteDbService<AppSettingsModel>(configuration);
var dbProvider = liteDbService
    .GetAll()
    .FirstOrDefault(x => x.KeyValueType == KeyValueType.DatabaseProvider);
var dbConnectionString = liteDbService
    .GetAll()
    .FirstOrDefault(x => x.KeyValueType == KeyValueType.ConnectionString);

if (dbProvider != null 
    && dbConnectionString != null 
    && dbProvider.DatabaseProvider != DatabaseProviderType.None 
    && !string.IsNullOrWhiteSpace(dbConnectionString.Value))
{
    switch (dbProvider.DatabaseProvider)
    {
        case DatabaseProviderType.SqlServer:
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(dbConnectionString.Value));
            break;
        case DatabaseProviderType.MySQL:
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseMySQL(dbConnectionString.Value));
            break;
        case DatabaseProviderType.PostgreSQL:
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseNpgsql(dbConnectionString.Value));
            break;
        default:
            break;
    }
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseExceptionHandler(configuration["ExceptionPage"]);
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization();

app.MapControllers();

app.UseSession();

app.MapBlazorHub(opt =>
{
    opt.LongPolling.PollTimeout = new TimeSpan(0, 30, 0);
    opt.WebSockets.CloseTimeout = new TimeSpan(0, 30, 0);
});
app.MapRazorPages();
app.MapFallbackToPage(configuration["FallbackPage"]);

app.Run();
