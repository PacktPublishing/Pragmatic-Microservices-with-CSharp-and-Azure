using BlazorApplicationInsights;
using Codebreaker.GameAPIs.Client;
using CodeBreaker.Blazor.Extensions;
using CodeBreaker.Blazor.UI;
using CodeBreaker.Blazor.UI.Services.Dialog;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLocalization();
builder.Services.AddBlazorApplicationInsights();
builder.Services.AddCodeBreakerUI();

builder.Services.AddHttpClient<IGamesClient, GamesClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBase"] ?? throw new InvalidOperationException("Could not read ApiBase"));
});
builder.Services.AddScoped<IDialogService, DialogService>();

var host = builder.Build();

await host.ConfigureLocalizationAsync();

await host.RunAsync();