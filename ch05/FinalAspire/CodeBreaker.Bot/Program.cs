using System.Runtime.CompilerServices;
using CodeBreaker.Bot.Endpoints;

[assembly: InternalsVisibleTo("CodeBreaker.Bot.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Swagger & EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient & Application Services

builder.Services.AddHttpClient<GamesClient>(client =>
{
    client.BaseAddress = new Uri("http://gameapis");
}).AddStandardResilienceHandler(config =>
{
    TimeSpan timeSpan = TimeSpan.FromMinutes(5);
    config.AttemptTimeout.Timeout = timeSpan;
    config.CircuitBreaker.SamplingDuration = timeSpan * 2;
    config.TotalRequestTimeout.Timeout = timeSpan * 3;
});
builder.Services.AddScoped<CodeBreakerTimer>();
builder.Services.AddScoped<CodeBreakerGameRunner>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();
app.MapBotEndpoints();

app.Run();
