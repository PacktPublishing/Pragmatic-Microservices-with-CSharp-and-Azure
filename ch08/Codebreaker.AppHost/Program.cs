var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var appConfig = builder.AddAzureAppConfiguration("codebreakerconfig");

var keyVault = builder.AddAzureKeyVault("codebreakervault");

var initAppConfig = builder.AddProject<Projects.Codebreaker_InitalizeAppConfig>("initappconfig")
    .WithReference(appConfig)
    .WaitFor(appConfig);

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var createCosmos = builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
    .WithReference(cosmos)
    .WaitFor(cosmos);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
  .WithReference(appConfig)
  .WithEnvironment("DataStore", dataStore)
  .WaitForCompletion(createCosmos);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(appConfig)
    .WithReference(gameAPIs)
    .WaitFor(appConfig)
    .WaitFor(gameAPIs);



builder.Build().Run();
