using AdaptiveSummaryGenerator.UI.Components;
using AdaptiveSummaryGenerator.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Shared app state ──────────────────────────────────────────────────────────
builder.Services.AddScoped<AppState>();

// ── Summary Service ───────────────────────────────────────────────────────────
// OPTION A — Use dummy data locally (no backend needed):
// builder.Services.AddScoped<ISummaryService, DummySummaryService>();

// OPTION B — Use real backend (ACTIVE). Backend must be running on port 5183.
// Base URL is read from appsettings.json → "BackendApi:BaseUrl"
builder.Services.AddHttpClient<ISummaryService, RealSummaryService>(client =>
{
    var baseUrl = builder.Configuration["BackendApi:BaseUrl"]
                  ?? "http://localhost:5183/";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
