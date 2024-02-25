// Copyright (C) 2024 Peter Bakota
// 
// Invoicer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Invoicer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with Invoicer. If not, see <http://www.gnu.org/licenses/>.

using Invoicer.Data.Seeding;
using Invoicer.Data.Dao;

using Radzen;
using Invoicer.App.Services;
using QuestPDF.Infrastructure;
using Invoicer.App.Reports;
using Invoicer.App;
using Invoicer.App.Configuration;
using Invoicer.App.Components.Account;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Invoicer.Models;
using Invoicer.Data;

using BlazorApp = Invoicer.App.Components.App;

#if ELECTRON_APP
using ElectronNET.API;
#endif

var builder = WebApplication.CreateBuilder(args);
var logger = LoggerFactory.Create(configure =>
{
    configure.AddConsole();
    configure.SetMinimumLevel(LogLevel.Trace);
}).CreateLogger<Program>();

#if ELECTRON_APP
logger.LogInformation("Starting electron app ... Environment: {}", Environment.GetEnvironmentVariable("_ENVIRONMENT"));
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();
#endif

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

// Authentication
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// DB connection
builder.AddInvoicerContext();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(options => options.DefaultScheme = IdentityConstants.ApplicationScheme).AddIdentityCookies();
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<InvoicerContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Repositories
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

// Services
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPrepayedService, PrepayedService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAppNotificationService, AppNotificationService>();

// Reports
builder.Services.AddScoped<IInvoiceReport, InvoiceReport>();

// Seeding and db preparing service
builder.Services.AddScoped<Seeder>();

// Localization
builder.Services.AddLocalization(/*opts => opts.ResourcesPath = "Resources"*/);

var supportedCultures = new[] { "sr", "en-US" };
builder.Services.Configure<RequestLocalizationOptions>(
    options => options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<BlazorApp>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

#if ELECTRON_APP
app.Use(async (context, next) =>
{
    context.Request.Headers.AcceptLanguage = "sr";
    await next.Invoke();
});
#endif

app.UseRequestLocalization();

using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();

#if ELECTRON_APP
if (Environment.GetEnvironmentVariable("_ENVIRONMENT") == "Development")
#else
if (app.Environment.IsDevelopment())
#endif
{
    logger.LogInformation("Preparing db for development ...");
    await seeder.SeedDevelopment();
}
else
{
    logger.LogInformation("Preparing db for production ...");
    await seeder.SeedProduction();
}

QuestPDF.Settings.License = LicenseType.Community;

#if ELECTRON_APP
Electron.Menu.SetApplicationMenu(ElectronMenu.Build(app));

await app.StartAsync();
static async Task CreateWindow()
{
    var window = await Electron.WindowManager.CreateWindowAsync();
    window.OnClosed += () => Electron.App.Quit();
}
await CreateWindow();
app.WaitForShutdown();

#else

app.Run();

#endif
