using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using Codebreaker.Data.Sqlite;
using Codebreaker.GameAPIs.Data;
using Codebreaker.GameAPIs.Data.InMemory;

using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Codbreaker.APIs.Tests")]

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Application Services

string dataStorage = builder.Configuration["DataStorage"] ??= "InMemory";

if (dataStorage == "Sqlite")
{
    builder.Services.AddDbContext<IGamesRepository, GamesSqliteContext>(options =>
    {
        string connectionString = builder.Configuration.GetConnectionString("GamesSqliteConnection") ?? throw new InvalidOperationException("Could not find GamesSqliteConnection");
        options.UseSqlite(connectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
}
else
{
    builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
}

builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

// -------------------------
// Endpoints
// -------------------------

app.MapGameEndpoints();
app.MapCreateDatabaseEndpoints(app.Logger);

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