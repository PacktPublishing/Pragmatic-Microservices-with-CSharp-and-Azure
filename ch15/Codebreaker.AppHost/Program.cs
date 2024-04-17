using Microsoft.Extensions.Azure;

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string startupMode = builder.Configuration["STARTUP_MODE"] ?? "Azure";
string botLoop = builder.Configuration.GetSection("Bot")["Loop"] ?? "false";
string botDelay = builder.Configuration.GetSection("Bot")["Delay"] ?? "1000";

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .PublishAsContainer();

if (startupMode == "OnPremises")
{
    var kafka = builder.AddKafka("kafkamessaging");

    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume()
        .AddDatabase("CodebreakerSql");

    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithExternalHttpEndpoints()
        .WithReference(sqlServer)
        .WithReference(redis)
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

    builder.AddProject<Projects.Codebreaker_Ranking>("ranking")
        .WithExternalHttpEndpoints()
        .WithReference(cosmos)
        .WithReference(kafka)
        .WithEnvironment("StartupMode", startupMode);
}
else
{
    var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
    var insights = builder.AddAzureApplicationInsights("insights", logs);
    var signalR = builder.AddAzureSignalR("signalr");
    var storage = builder.AddAzureStorage("storage");

    var botQueue = storage.AddQueues("botqueue");
    var blob = storage.AddBlobs("checkpoints");

    var eventHub = builder.AddAzureEventHubs("codebreakerevents")
        .AddEventHub("games");

    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithExternalHttpEndpoints()
        .WithReference(cosmos)
        .WithReference(redis)
        .WithReference(insights)
        .WithReference(eventHub)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("StartupMode", startupMode);

    builder.AddProject<Projects.Codebreaker_BotQ>("bot")
        .WithReference(insights)
        .WithReference(botQueue)
        .WithReference(gameAPIs)
        .WithEnvironment("Bot__Loop", botLoop)
        .WithEnvironment("Bot__Delay", botDelay);

    var live = builder.AddProject<Projects.Codebreaker_Live>("live")
        .WithExternalHttpEndpoints()
        .WithReference(insights)
        .WithReference(eventHub)
        .WithReference(signalR);

    builder.AddProject<Projects.Codebreaker_Ranking>("ranking")
        .WithExternalHttpEndpoints()
        .WithReference(cosmos)
        .WithReference(insights)
        .WithReference(eventHub)
        .WithReference(blob);
}

builder.Build().Run();
