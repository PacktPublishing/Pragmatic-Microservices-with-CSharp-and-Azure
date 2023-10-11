using System.Runtime.CompilerServices;

using Azure.Core.Diagnostics;
using Azure.Identity;

using Codebreaker.Data.Cosmos;
using Codebreaker.Data.SqlServer;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

[assembly: InternalsVisibleTo("Codbreaker.APIs.Tests")]

var builder = WebApplication.CreateBuilder(args);

string? solutionEnvironment = builder.Configuration["SolutionEnvironment"];

#if DEBUG
using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

DefaultAzureCredentialOptions credentialOptions = new()
{
    Diagnostics =
    {
        LoggedHeaderNames = { "x-ms-request-id" },
        LoggedQueryParameters = { "api-version" },
        IsLoggingContentEnabled = true
    },
    ExcludeSharedTokenCacheCredential = true,
    ExcludeAzurePowerShellCredential = true,
    ExcludeVisualStudioCodeCredential = true,
    ExcludeEnvironmentCredential = true,
    ExcludeInteractiveBrowserCredential = true,
    ExcludeAzureCliCredential = false,
    ExcludeManagedIdentityCredential = true,
    ExcludeVisualStudioCredential = true
};

DefaultAzureCredential credential = new(credentialOptions);
#else
DefaultAzureCredential credential = new();
#endif

if (solutionEnvironment == "Azure")
{
    string endpoint = builder.Configuration["AzureAppConfigurationUri"] ?? throw new InvalidOperationException("Could not read AzureAppConfigurationUri");

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(endpoint), credential);
    });
}

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo
    {
        Version = "v3",
        Title = "Codebreaker Games API",
        Description = "An ASP.NET Core minimal APIs to play Codebreaker games",
        TermsOfService = new Uri("https://www.cninnovation.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Christian Nagel",
            Url = new Uri("https://csharp.christiannagel.com")
        },
        License = new OpenApiLicense
        {
            Name = "License API Usage",
            Url = new Uri("https://www.cninnovation.com/apiusage")
        }
    });
});

// Application Services

string dataStorage = builder.Configuration["DataStorage"] ??= "Cosmos";

if (dataStorage == "Cosmos")
{
    builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
    {
        string cosmosEndpoint = builder.Configuration.GetSection("GamesAPI").GetConnectionString("GamesCosmosEndpoint") ?? throw new InvalidOperationException("Could not read GamesCosmosEndpoint");
        options.UseCosmos(cosmosEndpoint, credential, databaseName: "codebreaker")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
}
else if (dataStorage == "SqlServer")
{
    builder.Services.AddDbContext<IGamesRepository, GamesSqlServerContext>(options =>
    {
        string connectionString = builder.Configuration.GetConnectionString("GamesSqlServerConnection") ?? throw new InvalidOperationException("Could not find GamesSqlServerConnection");
        options.UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
}
else
{
    builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
}

builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
    });
}

app.MapGameEndpoints();

if (bool.TryParse(builder.Configuration["AllowCreateDatabase"], out bool allowCreateDatabase) && allowCreateDatabase)
{
    app.MapCreateDatabaseEndpoints(app.Logger);
}

app.Run();
