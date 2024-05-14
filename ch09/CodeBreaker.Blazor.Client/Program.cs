using BlazorApplicationInsights;
using Codebreaker.GameAPIs.Client;
using CodeBreaker.Blazor.Client.Extensions;
using CodeBreaker.Blazor.UI;
using CodeBreaker.Blazor.UI.Services.Dialog;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLocalization();
builder.Services.AddBlazorApplicationInsights();
builder.Services.AddCodeBreakerUI();

builder.Services.AddHttpClient("GameApi",  (HttpClient client) =>
    client.BaseAddress = new Uri(builder.Configuration.GetRequired("ApiBase")));

builder.Services.AddHttpClient<IGamesClient, GamesClient>("GameApi");
builder.Services.AddScoped<IDialogService, DialogService>();

var host = builder.Build();

await host.ConfigureLocalizationAsync();

await host.RunAsync();