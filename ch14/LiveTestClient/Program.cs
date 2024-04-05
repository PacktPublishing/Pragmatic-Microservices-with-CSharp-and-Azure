
Console.WriteLine("Test client - wait for service, then press return to continue");
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
