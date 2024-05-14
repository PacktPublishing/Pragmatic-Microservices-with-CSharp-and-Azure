var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var appConfig = builder.AddAzureAppConfiguration("codebreakerconfig");

var keyVault = builder.AddAzureKeyVault("codebreakervault");

builder.AddProject<Projects.Codebreaker_InitalizeAppConfig>("initappconfig")
    .WithReference(appConfig);

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
  .WithReference(appConfig)
  .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(appConfig)
    .WithReference(gameAPIs);

builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
    .WithReference(cosmos);

builder.Build().Run();
