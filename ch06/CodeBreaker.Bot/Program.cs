using System.Runtime.CompilerServices;
using CodeBreaker.Bot.Endpoints;

[assembly: InternalsVisibleTo("MMBot.Tests")]

var builder = WebApplication.CreateBuilder(args);

WebApplication? app = null;

// Swagger & EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient & Application Services
builder.Services.AddHttpClient<GamesClient>(options =>
{
    string codebreakeruri = builder.Configuration["ApiBase"]
        ?? throw new InvalidOperationException("ApiBase configuration not available");

    var apiUri = new Uri(codebreakeruri);

    options.BaseAddress = apiUri;
});
builder.Services.AddScoped<CodeBreakerTimer>();
builder.Services.AddScoped<CodeBreakerGameRunner>();

app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapBotEndpoints();

app.Run();
