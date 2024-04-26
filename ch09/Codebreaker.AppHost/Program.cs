var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var cosmos = builder.AddAzureCosmosDB("cbcosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
  .WithReference(cosmos)
  .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

// currently disabled - see https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/issues/81
//builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
//    .WithReference(cosmos);

//builder.AddProject<Projects.Codebreaker_SqlServerMigration>("sqlcreate")
//    .WithReference(sqlServer);

builder.Build().Run();
