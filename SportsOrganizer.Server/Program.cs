using Append.Blazor.Printing;
using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Interfaces;
using SportsOrganizer.Server.Models;
using SportsOrganizer.Server.Services;
using SportsOrganizer.Server.Utils;

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
builder.Services.AddScoped<DialogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<CultureProviderService>();
builder.Services.AddScoped<MemoryStorageUtility>();
builder.Services.AddScoped<IPrintingService, PrintingService>();

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

builder.Services.AddScoped<ApplicationDbContextService>();
builder.Services.AddScoped<AuthenticatorService>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticatorService>());

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

app.MapBlazorHub(opt =>
{
    opt.LongPolling.PollTimeout = new TimeSpan(0, 30, 0);
    opt.WebSockets.CloseTimeout = new TimeSpan(0, 30, 0);
});
app.MapRazorPages();
app.MapFallbackToPage(configuration["FallbackPage"]);

app.Run();
