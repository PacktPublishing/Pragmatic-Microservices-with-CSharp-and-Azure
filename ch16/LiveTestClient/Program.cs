
Console.WriteLine("Test client - wait for service, then press return to continue");
Console.ReadLine();

CancellationTokenSource cts = new();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<StreamingLiveClient>();
builder.Services.Configure<LiveClientOptions>(builder.Configuration.GetSection("Codebreaker.Live"));
using var host = builder.Build();

var client = host.Services.GetRequiredService<StreamingLiveClient>();
await client.StartMonitorAsync(cts.Token);
await client.SubscribeToGame("Game6x4", cts.Token);

await host.RunAsync();

cts.Cancel();
Console.WriteLine("Bye...");
