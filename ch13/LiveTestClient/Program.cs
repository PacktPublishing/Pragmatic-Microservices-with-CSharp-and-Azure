using LiveTestClient;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Test client - wait for service");
Console.ReadLine();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<LiveClient>();
builder.Services.Configure<LiveClientOptions>(builder.Configuration.GetSection("Codebreaker.Live"));
using var host = builder.Build();

var client = host.Services.GetRequiredService<LiveClient>();
await client.StartMonitorAsync();
await client.SubscribeToGame("Game6x4");

await host.RunAsync();

Console.WriteLine("Bye...");
