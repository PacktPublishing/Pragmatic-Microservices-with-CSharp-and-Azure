using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Configuration.AddJsonFile("connectionstrings.json", optional: true);

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
        .Select("ConfigurationPrototype*")
        .ConfigureKeyVault(keyVaultOptions =>
        {
            keyVaultOptions.SetCredential(credential);
        });
});

// Add services to the container.

builder.Services.Configure<Service1Options>(builder.Configuration.GetSection("Service1"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/readconfig", (IConfiguration config) =>
{
    string? config1 = config["Config1"];
    return $"config1: {config1}";
});

app.MapGet("/readoptions", (IOptions<Service1Options> options) =>
{
    return $"options - config1: {options.Value.Config1}; config 2: {options.Value.Config2}";
});

app.MapGet("/azureconfig", (IConfiguration config) =>
{
    string? connectionString = config.GetSection("ConfigurationPrototype").GetConnectionString("SqlServer");
    return $"Configuration value from Azure App Configuration: {connectionString}";
});

app.MapGet("/secret", (IConfiguration config) =>
{
    string? connectionString = config.GetSection("ConfigurationPrototype").GetConnectionString("Cosmos");
    return $"Configuration value from Azure Key Vault via App Configuration: {connectionString}";
});

app.Run();

internal class Service1Options
{
    public required string Config1 { get; set; }
    public string? Config2 { get; set; }
}