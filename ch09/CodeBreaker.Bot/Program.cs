using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using CodeBreaker.Bot.Endpoints;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var codebreakerBotSection = builder.Configuration.GetSection("CodebreakerBot");
string? solutionEnvironment = codebreakerBotSection["SolutionEnvironment"];
string? authentication = codebreakerBotSection["Authentication"];
string? gamesapiScope = codebreakerBotSection["GamesAPIScope"];

string? managedIdentityClientId = codebreakerBotSection["ManagedIdentityClientId"];

if (authentication == "AADB2C")
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>
        {
            // bearer options
            codebreakerBotSection.Bind("AzureAdB2C", options);

            options.TokenValidationParameters.NameClaimType = "name";
        }, options =>
        {  // identity options
            codebreakerBotSection.Bind("AzureADB2C", options);
        })
        .EnableTokenAcquisitionToCallDownstreamApi(options =>
        {

        }).AddDownstreamApi("gamesapi", builder.Configuration.GetSection("GamesAPIScope"))
        .AddInMemoryTokenCaches();
   

    var authorizationBuilder = builder.Services.AddAuthorizationBuilder();
    authorizationBuilder.AddPolicy("BotPolicy", config =>
    {
        config.RequireAuthenticatedUser().RequireRole("BotUsers");
        config.Requirements.Add(
            new ScopeAuthorizationRequirement()
            {

            });
    });

}

if (solutionEnvironment == "Azure")
{
    DefaultAzureCredentialOptions credentialOptions = new()
    {
        ManagedIdentityClientId = managedIdentityClientId
    };
    DefaultAzureCredential credential = new(credentialOptions);

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

app.UseAuthentication();
app.UseAuthorization();

//app.MapSwagger().RequireAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapBotEndpoints();

app.Run();
