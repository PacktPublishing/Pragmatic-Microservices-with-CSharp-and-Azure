using CodeBreaker.Blazor.Pages;
using CodeBreaker.Blazor.UI;
using CodeBreaker.Blazor.UI.Services.Dialog;
using Codebreaker.GameAPIs.Client;
using CodeBreaker.Blazor.Host.Components;
using CodeBreaker.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLocalization();
builder.Services.AddCodeBreakerUI();

//builder.Services.AddHttpClient("GameApi", configure =>
//    configure.BaseAddress = new Uri(builder.Configuration.GetRequired("ApiBase")));

//builder.Services.AddHttpClient<IGamesClient, GamesClient>("GameApi");
builder.Services.AddScoped<IDialogService, DialogService>();

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(["de", "en"])
    .AddSupportedUICultures(["de", "en"]));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
   // .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GamePage).Assembly);

app.MapGet("/gamesAPIUrl", () =>
{
    string? url = Environment.GetEnvironmentVariable("GameAPIs");
    return TypedResults.Ok(url);
});

app.Run();