using Azure.Identity;

using CodeBreaker.Bot.Endpoints;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

string? solutionEnvironment = builder.Configuration["SolutionEnvironment"];

if (solutionEnvironment == "Azure")
{
    DefaultAzureCredential credential = new();

    string endpoint = builder.Configuration["AzureAppConfigurationUri"] ?? throw new InvalidOperationException("Could not read AzureAppConfigurationUri");

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(endpoint), credential)
            .Select("BotService*", labelFilter: LabelFilter.Null)
            .Select("BotService*", builder.Environment.EnvironmentName);
    });
}

WebApplication? app = null;

// Swagger & EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient & Application Services
builder.Services.AddHttpClient<GamesClient>(options =>
{
    string codebreakeruri = builder.Configuration.GetSection("BotService")["ApiBase"]
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
