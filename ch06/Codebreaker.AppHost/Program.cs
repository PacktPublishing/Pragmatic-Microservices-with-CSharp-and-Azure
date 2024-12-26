var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var createCosmos = builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
    .WithReference(cosmos)
    .WaitFor(cosmos);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos)
    .WithEnvironment("DataStore", dataStore)
    .WaitForCompletion(createCosmos)
    .WaitFor(cosmos);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithExternalHttpEndpoints()
    .WithReference(gameAPIs)
    .WaitFor(gameAPIs);

builder.Build().Run();
