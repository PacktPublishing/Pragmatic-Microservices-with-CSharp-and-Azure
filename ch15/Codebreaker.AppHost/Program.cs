

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string botLoop = builder.Configuration.GetSection("Bot")["Loop"] ?? "false";
string botDelay = builder.Configuration.GetSection("Bot")["Delay"] ?? "1000";

builder.AddAzureProvisioning();

var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
var insights = builder.AddAzureApplicationInsights("insights", logs);
var signalR = builder.AddAzureSignalR("signalr");
var queue = builder.AddAzureStorage("storage")
    .AddQueues("botqueue");

var eventHub = builder.AddAzureEventHubs("codebreakerevents")
    .AddEventHub("games");

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .PublishAsContainer();

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var live = builder.AddProject<Projects.Codebreaker_Live>("live", "https")
    .WithExternalHttpEndpoints()
    .WithReference(insights)
    .WithReference(eventHub)
    .WithReference(signalR);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis", "https")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos)
    .WithReference(redis)
    .WithReference(insights)
    .WithReference(live)
    .WithReference(eventHub)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.Codebreaker_BotQ>("bot")
    .WithReference(insights)
    .WithReference(queue)
    .WithReference(gameAPIs)
    .WithEnvironment("Bot__Loop", botLoop)
    .WithEnvironment("Bot__Delay", botDelay);

builder.AddProject<Projects.Codebreaker_Ranking>("ranking")
    .WithReference(cosmos)
    .WithExternalHttpEndpoints()
    .WithReference(insights)
    .WithReference(eventHub);

builder.Build().Run();
