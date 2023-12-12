using System.Data.Common;
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
string? managedIdentityClientId = builder.Configuration["ManagedIdentityClientId"];

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
    ExcludeManagedIdentityCredential = false,
    ExcludeVisualStudioCredential = false
};

DefaultAzureCredential credential = new(credentialOptions);
#else
DefaultAzureCredentialOptions credentialOptions = new()
{
    ManagedIdentityClientId = managedIdentityClientId,
    ExcludeSharedTokenCacheCredential = true,
    ExcludeAzurePowerShellCredential = true,
    ExcludeVisualStudioCodeCredential = true,
    ExcludeEnvironmentCredential = true,
    ExcludeInteractiveBrowserCredential = true,
    ExcludeAzureCliCredential = false,
    ExcludeManagedIdentityCredential = false,
    ExcludeVisualStudioCredential = false
};

DefaultAzureCredential credential = new(credentialOptions);
#endif

if (solutionEnvironment == "Azure")
{
    string endpoint = builder.Configuration["AzureAppConfigurationUri"] ?? throw new InvalidOperationException("Could not read AzureAppConfigurationUri");

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(endpoint), credential)
            .Select("GamesAPI*")
            .ConfigureKeyVault(kv =>
            {
                kv.SetCredential(credential);
            });
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

string dataStorage = builder.Configuration.GetSection("GamesAPI")["DataStorage"] ??= "InMemory";

if (dataStorage == "Cosmos")
{
    builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
    {
        // this configuration is using the secret from the Key Vault via the Azure App Configuration
        string cosmosConnectionString = builder.Configuration.GetSection("GamesAPI").GetConnectionString("CosmosConnectionString") ?? throw new InvalidOperationException("Could not read CosmosConnectionString");
        options.UseCosmos(cosmosConnectionString, databaseName: "codebreaker")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        //string cosmosEndpoint = builder.Configuration.GetSection("GamesAPI").GetConnectionString("GamesCosmosEndpoint") ?? throw new InvalidOperationException("Could not read GamesCosmosEndpoint");
        //options.UseCosmos(cosmosEndpoint, credential, databaseName: "codebreaker")
        //    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
}
else if (dataStorage == "SqlServer")
{
    builder.Services.AddDbContext<IGamesRepository, GamesSqlServerContext>(options =>
    {
        string connectionString = builder.Configuration.GetConnectionString("GamesSqlServerConnection") ?? throw new InvalidOperationException("Could not find GamesSqlServerConnection");
        DbConnectionStringBuilder connectionStringBuilder = new()
        {
            ConnectionString = connectionString
        };
        if (connectionStringBuilder.ContainsKey("user id"))
        {
            if (File.Exists("/run/secrets/sqlpassword"))
            {
                string password = File.ReadAllText("/run/secrets/sqlpassword");
                connectionStringBuilder.Add("password", password);
            }
        }

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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

app.MapGameEndpoints();

app.Run();
