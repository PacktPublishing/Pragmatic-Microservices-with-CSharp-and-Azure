using Codebreaker.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// open appsettings.json and appsettings.Development.json to set the settings
string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string useEmulator = builder.Configuration["UseEmulator"] ?? "PreferDocker";  // options: PreferDocker, PreferLocal, UseAzure
string startupMode = builder.Configuration["STARTUP_MODE"] ?? "Azure";
string cache = builder.Configuration["Cache"] ?? "Redis";  // options: Redis, Garnet, None

string botLoop = builder.Configuration.GetSection("Bot")["Loop"] ?? "false";
string botDelay = builder.Configuration.GetSection("Bot")["Delay"] ?? "1000";

IResourceBuilder<ProjectResource> gameAPIs;
IResourceBuilder<ProjectResource> ranking;

if (startupMode == "OnPremises")
{
    var kafka = builder.AddKafka("kafkamessaging");

    gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithExternalHttpEndpoints()
        .WithReference(kafka)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("StartupMode", startupMode);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithExternalHttpEndpoints()
        .WithReference(gameAPIs)
        .WithEnvironment("Bot__Loop", botLoop)
        .WithEnvironment("Bot__Delay", botDelay);

    var live = builder.AddProject<Projects.Codebreaker_Live>("live")
        .WithExternalHttpEndpoints()
        .WithReference(kafka)
        .WithEnvironment("StartupMode", startupMode);

    ranking = builder.AddProject<Projects.Codebreaker_Ranking>("ranking")
        .WithExternalHttpEndpoints()
        .WithReference(kafka)
        .WithEnvironment("StartupMode", startupMode);
}
else // Azure
{
#pragma warning disable ASPIREAZURE001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    builder.AddAzurePublisher();
#pragma warning restore ASPIREAZURE001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
    var insights = builder.AddAzureApplicationInsights("insights", logs);
    var signalR = builder.AddAzureSignalR("signalr");

    var storage = builder.AddCodebreakerStorage(useEmulator == "PreferDocker");
 
    var eventHub = builder.AddCodebreakerEventHub(useEmulator != "UseAzure");

    gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithExternalHttpEndpoints()
        .WithReference(insights)
        .WithReference(eventHub)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("StartupMode", startupMode)
        .WithEnvironment("Cache", cache)
        .WaitFor(insights)
        .WaitFor(eventHub);

    builder.AddProject<Projects.Codebreaker_BotQ>("bot")
        .WithReference(insights)
        .WithReference(storage.BotQueue)
        .WithReference(gameAPIs)
        .WithEnvironment("Bot__Loop", botLoop)
        .WithEnvironment("Bot__Delay", botDelay)
        .WaitFor(gameAPIs);

    var live = builder.AddProject<Projects.Codebreaker_Live>("live")
        .WithExternalHttpEndpoints()
        .WithReference(insights)
        .WithReference(eventHub)
        .WithReference(signalR)
        .WaitFor(gameAPIs);

    ranking = builder.AddProject<Projects.Codebreaker_Ranking>("ranking")
        .WithExternalHttpEndpoints()
        .WithReference(insights)
        .WithReference(eventHub)
        .WithReference(storage.Blob)
        .WaitFor(storage.Blob)
        .WaitFor(eventHub);

    var liveClient = builder.AddProject<Projects.Codebreaker_Live_Blazor>("liveclient")
        .WithExternalHttpEndpoints()
        .WithReference(insights)
        .WithReference(live)
        .WaitFor(live);
}

if (dataStore == "Cosmos")
{
    var cosmos = builder.AddCodebreakerCosmos(useEmulator == "PreferDocker");

    gameAPIs.WithReference(cosmos.GamesContainer)
        .WaitFor(cosmos.GamesContainer);

    ranking.WithReference(cosmos.RankingContainer)
        .WaitFor(cosmos.RankingContainer);

}
else if (dataStore == "SqlServer")
{
    var sqlServer = builder.AddCodebreakerSqlServer(useEmulator == "PreferDocker");

    gameAPIs.WithReference(sqlServer)
        .WaitFor(sqlServer);

}
else if (dataStore == "Postgres")
{

}
else
{
    // in-memory, no integration is needed
}

if (cache == "Redis")
{
    var redis = builder.AddCodebreakerRedis();

    gameAPIs.WithReference(redis)
        .WaitFor(redis);
}
else if (cache == "Garnet")
{
    var garnet = builder.AddCodebreakerGarnet();

    gameAPIs.WithReference(garnet)
        .WaitFor(garnet);
}

builder.Build().Run();
