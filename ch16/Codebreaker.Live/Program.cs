using Codebreaker.Live;

using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapApplicationEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

//var processor = app.Services.GetRequiredService<LiveGamesEventProcessor>();
//await processor.StartProcessingAsync();

app.Run();
