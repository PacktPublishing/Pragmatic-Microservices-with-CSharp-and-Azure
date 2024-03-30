using Azure.Identity;
using Codebreaker.GameAPIs;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.Configuration.AddAzureAppConfiguration(appConfigOptions =>
//{
//#if DEBUG
//    DefaultAzureCredential credential = new();
//#else
//    string managedIdentityClientId = builder.Configuration["AZURE_CLIENT_ID"] ?? string.Empty;
//    DefaultAzureCredentialOptions credentialOptions = new()
//    {
//        ManagedIdentityClientId = managedIdentityClientId,
//        ExcludeEnvironmentCredential = true,
//        ExcludeWorkloadIdentityCredential = true
//    };
//    DefaultAzureCredential credential = new(credentialOptions);
//#endif
//    string appConfigUrl = builder.Configuration.GetConnectionString("codebreakerconfig") ?? throw new InvalidOperationException("could not read codebreakerconfig");
//    appConfigOptions.Connect(new Uri(appConfigUrl), credential)
//        .Select("gameapis")
//        .ConfigureKeyVault(keyVaultOptions =>
//        {
//            keyVaultOptions.SetCredential(credential);
//        });
//});

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo
    {
        Version = "v3",
        Title = "Codebreaker Games API",
        Description = "An ASP.NET Core minimal API to play Codebreaker games",
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

builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

app.MapGameEndpoints();

await app.CreateOrUpdateDatabaseAsync();

app.Run();
