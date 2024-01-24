using System.Text.Json.Serialization;

using Codebreaker.GameAPIs;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Application Services

builder.AddApplicationServices();

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
