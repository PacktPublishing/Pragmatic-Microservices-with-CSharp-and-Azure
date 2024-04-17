

var builder = DistributedApplication.CreateBuilder(args);


string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

builder.AddAzureProvisioning();

var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
var insights = builder.AddAzureApplicationInsights("insights", logs);
var signalR = builder.AddAzureSignalR("signalr");

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .PublishAsContainer();

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var live = builder.AddProject<Projects.Codebreaker_Live>("live", "https")
    .WithExternalHttpEndpoints()
    .WithReference(insights)
    .WithReference(signalR);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis", "https")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos)
    .WithReference(redis)
    .WithReference(insights)
    .WithReference(live)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithExternalHttpEndpoints()
    .WithReference(insights)
    .WithReference(gameAPIs);

builder.Build().Run();
