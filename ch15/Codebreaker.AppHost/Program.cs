

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string botLoop = builder.Configuration.GetSection("Bot")["Loop"] ?? "false";
string botDelay = builder.Configuration.GetSection("Bot")["Delay"] ?? "1000";

builder.AddAzureProvisioning();

var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
var insights = builder.AddAzureApplicationInsights("insights", logs);
var signalR = builder.AddAzureSignalR("signalr");
var storage = builder.AddAzureStorage("storage");

var botQueue = storage.AddQueues("botqueue");
var blob = storage.AddBlobs("checkpoints");

var eventHub = builder.AddAzureEventHubs("codebreakerevents")
    .AddEventHub("games");

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .PublishAsContainer();

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

builder.AddProject<Projects.Codebreaker_Live>("live")
    .WithExternalHttpEndpoints()
    .WithReference(insights)
    .WithReference(eventHub)
    .WithReference(signalR);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos)
    .WithReference(redis)
    .WithReference(insights)
    .WithReference(eventHub)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.Codebreaker_BotQ>("bot")
    .WithReference(insights)
    .WithReference(botQueue)
    .WithReference(gameAPIs)
    .WithEnvironment("Bot__Loop", botLoop)
    .WithEnvironment("Bot__Delay", botDelay);

builder.AddProject<Projects.Codebreaker_Ranking>("ranking")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos)
    .WithReference(insights)
    .WithReference(eventHub)
    .WithReference(blob);

if (builder.Environment.EnvironmentName == "OnPremises")
{
    
}

builder.Build().Run();
