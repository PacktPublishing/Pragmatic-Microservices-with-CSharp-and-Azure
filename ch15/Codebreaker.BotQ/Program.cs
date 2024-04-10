
using Codebreaker.BotQ.Endpoints;

var builder = Host.CreateApplicationBuilder(args);
builder.AddApplicationServices();

var app = builder.Build();

BotQueueClient botQueueClient = app.Services.GetRequiredService<BotQueueClient>();
await botQueueClient.RunAsync();

// app.Run();

