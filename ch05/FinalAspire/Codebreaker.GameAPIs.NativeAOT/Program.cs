using System.Text.Json.Serialization;

using Codebreaker.Data.Sqlite;
using Codebreaker.GameAPIs.Data.InMemory;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Application Services

string dataStorage = builder.Configuration["DataStorage"] ??= "InMemory";

var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger("Program");
if (dataStorage == "Sqlite")
{
    logger.LogInformation("Using Sqlite");
    builder.Services.AddScoped<IGamesRepository>(provider =>
    {
        string connectionString = builder.Configuration.GetConnectionString("GamesSqliteConnection") ?? throw new InvalidOperationException("Could not find GamesSqliteConnection");
        return new GamesSqliteContext(connectionString);
    });

    //builder.Services.AddDbContext<IGamesRepository, GamesSqliteContext>(options =>
    //{
    //    string connectionString = builder.Configuration.GetConnectionString("GamesSqliteConnection") ?? throw new InvalidOperationException("Could not find GamesSqliteConnection");
    //    options.UseSqlite(connectionString)
    //        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    //});
}
else
{
    logger.LogInformation("Using InMemory");
    builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
}

builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

// -------------------------
// Endpoints
// -------------------------

app.MapGameEndpoints();

app.Run();

[JsonSerializable(typeof(IEnumerable<Game>))]
[JsonSerializable(typeof(UpdateGameRequest))]
[JsonSerializable(typeof(UpdateGameResponse))]
[JsonSerializable(typeof(CreateGameResponse))]
[JsonSerializable(typeof(CreateGameRequest))]
[JsonSerializable(typeof(Game[]))]
[JsonSerializable(typeof(string[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}