using Codebreaker.Ranking;
using Codebreaker.Ranking.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.CreateOrUpdateDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.MapApplicationEndpoints();

var eventProcessor = app.Services.GetRequiredService<IGameSummaryProcessor>();
await eventProcessor.StartProcessingAsync();

app.Run();

await eventProcessor.StopProcessingAsync();