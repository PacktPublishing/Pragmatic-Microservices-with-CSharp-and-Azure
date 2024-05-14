var builder = DistributedApplication.CreateBuilder(args);

var dataStoreConfig = builder.Configuration["DataStore"] ?? "InMemory";

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(cosmos)
    .WithEnvironment("DataStore", dataStoreConfig);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.Build().Run();
