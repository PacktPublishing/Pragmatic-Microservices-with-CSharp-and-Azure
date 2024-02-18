using System.Runtime.CompilerServices;
using CodeBreaker.Bot.Endpoints;

[assembly: InternalsVisibleTo("CodeBreaker.Bot.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Swagger & EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient & Application Services


builder.Services.AddHttpClient<GamesClient>(client =>
{
    client.BaseAddress = new Uri("http://codebreaker.gameapis");
});
builder.Services.AddScoped<CodeBreakerTimer>();
builder.Services.AddScoped<CodeBreakerGameRunner>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapBotEndpoints();

app.Run();
