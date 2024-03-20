using System.Runtime.CompilerServices;
using Azure.Identity;
using CodeBreaker.Bot.Endpoints;

[assembly: InternalsVisibleTo("CodeBreaker.Bot.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddAzureAppConfiguration(appConfigOptions =>
{
#if DEBUG
    DefaultAzureCredential credential = new();
#else
    string managedIdentityClientId = builder.Configuration["AZURE_CLIENT_ID"] ?? string.Empty;
    DefaultAzureCredentialOptions credentialOptions = new()
    {
        ManagedIdentityClientId = managedIdentityClientId,
        ExcludeEnvironmentCredential = true,
        ExcludeWorkloadIdentityCredential = true
    };
    DefaultAzureCredential credential = new(credentialOptions);
#endif
    string appConfigUrl = builder.Configuration.GetConnectionString("codebreakerconfig") ?? throw new InvalidOperationException("could not read codebreakerconfig");
    appConfigOptions.Connect(new Uri(appConfigUrl), credential)
        .Select("bot")
        .ConfigureKeyVault(keyVaultOptions =>
        {
            keyVaultOptions.SetCredential(credential);
        });
});

// Swagger & EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient & Application Services
builder.AddApplicationServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();
app.MapBotEndpoints();

app.Run();
