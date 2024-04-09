using System.Runtime.CompilerServices;
using CodeBreaker.Bot.Endpoints;

[assembly: InternalsVisibleTo("CodeBreaker.Bot.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Swagger & EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient & Application Services
// builder.AddAppConfiguration();
builder.AddApplicationServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();
app.MapBotEndpoints();


app.Run();
