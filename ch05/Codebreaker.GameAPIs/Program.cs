using System.Data.Common;
using System.Runtime.CompilerServices;

using Codebreaker.Data.Cosmos;
using Codebreaker.Data.SqlServer;
using Codebreaker.GameAPIs.Data;
using Codebreaker.GameAPIs.Data.InMemory;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

[assembly: InternalsVisibleTo("Codbreaker.APIs.Tests")]

var builder = WebApplication.CreateBuilder(args);

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
            Name="License API Usage",
            Url= new Uri("https://www.cninnovation.com/apiusage")
        }
    });

});
builder.Services.AddProblemDetails();

// Application Services

string dataStorage = builder.Configuration["DataStorage"] ??= "Cosmos";

if (dataStorage == "Cosmos")
{
    builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
    {
        string connectionString = builder.Configuration.GetConnectionString("GamesCosmosConnection") ?? throw new InvalidOperationException("Could not find GamesCosmosConnection");
        options.UseCosmos(connectionString, databaseName: "codebreaker")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
            connectionString = connectionStringBuilder.ConnectionString;
        }

        Console.WriteLine($"using connection string {connectionString}");
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
    // options.InjectStylesheet("/swagger-ui/swaggerstyle.css");
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

// -------------------------
// Endpoints
// -------------------------

app.MapGameEndpoints();
app.MapCreateDatabaseEndpoints(app.Logger);

app.Run();
