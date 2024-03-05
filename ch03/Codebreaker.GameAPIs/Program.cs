
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

// TODO: temporary workaround - wait for Cosmos emulator to be available
if (app.Configuration["DataStore"] == "Cosmos")
{
    bool succeed = false;
    int maxRetries = 30;
    int i = 0;
    HttpClient client = new();
    string cosmosConnection = app.Configuration.GetConnectionString("codebreakercosmos");
    var ix1 = cosmosConnection.IndexOf("https");
    var ix2 = cosmosConnection.IndexOf(";DisableServer");
    string url = cosmosConnection[ix1..ix2];
    while (!succeed && i++ < maxRetries)
    {
        try
        {
            await Task.Delay(5000);
            await client.GetAsync(url);
            succeed = true;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, ex.Message);
            if (ex.InnerException is not null)
            {
                app.Logger.LogWarning(ex.InnerException, ex.InnerException.Message);
            }
        }
    }
}


app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

await app.CreateOrUpdateDatabaseAsync();

app.MapGameEndpoints();

app.Run();
