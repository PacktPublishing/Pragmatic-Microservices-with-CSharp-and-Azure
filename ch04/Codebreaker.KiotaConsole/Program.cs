using System.Text;

using Codebreaker.Client;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHttpClient<GamesClient>(client =>
//{
//    string gamesUrl = builder.Configuration["GamesApiUrl"] ?? throw new InvalidOperationException("GamesApiUrl not found");
//    client.BaseAddress = new Uri(gamesUrl);
//});

builder.Services.Configure<RunnerOptions>(options =>
{
    options.GamesApiUrl = builder.Configuration["GamesApiUrl"] ?? throw new InvalidOperationException("GamesApiUrl not found");
});

builder.Services.AddTransient<Runner>();
var app = builder.Build();

var runner = app.Services.GetRequiredService<Runner>();
await runner.RunAsync();
