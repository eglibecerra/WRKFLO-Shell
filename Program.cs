using System.Globalization;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using Wrkflo.Shell.Components;
using Wrkflo.Shell.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor
builder.Services.AddMudServices();

// Tenant branding (swap for DB-backed implementation later)
builder.Services.AddScoped<ITenantBrandingService, DefaultTenantBrandingService>();

// Localisation
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-AU"),
        new CultureInfo("en"),
        new CultureInfo("es"),
        new CultureInfo("fr"),
        new CultureInfo("de"),
        new CultureInfo("pt"),
        new CultureInfo("ja"),
        new CultureInfo("zh"),
    };
    options.DefaultRequestCulture = new RequestCulture("en-AU");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Use request localisation middleware
app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
