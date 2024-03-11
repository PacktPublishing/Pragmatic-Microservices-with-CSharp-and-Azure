using CodeBreaker.Blazor;
using CodeBreaker.Blazor.Components;
using CodeBreaker.Blazor.Client.Components;
using CodeBreaker.Blazor.Client.Pages;
using CodeBreaker.Blazor.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLocalization();
builder.Services.AddCodeBreakerUI();

builder.AddServiceDefaults();
builder.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(["de", "en"])
    .AddSupportedUICultures(["de", "en"]));

app.MapGet("config/gamesapi", () =>
{
    return Environment.GetEnvironmentVariable("http://gameapis") ?? throw new InvalidOperationException("Don't know about games URI");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GamePage).Assembly);

app.Run();