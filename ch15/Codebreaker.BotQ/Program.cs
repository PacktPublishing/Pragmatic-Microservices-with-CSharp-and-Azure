
using Codebreaker.BotQ.Endpoints;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddApplicationServices();

var app = builder.Build();

BotQueueClient botQueueClient = app.Services.GetRequiredService<BotQueueClient>();
await botQueueClient.RunAsync();
