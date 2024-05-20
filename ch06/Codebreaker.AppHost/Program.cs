
var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithExternalHttpEndpoints()
    .WithReference(gameAPIs);

builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
    .WithReference(cosmos);

builder.Build().Run();
