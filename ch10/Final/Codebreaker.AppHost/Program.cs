var builder = DistributedApplication.CreateBuilder(args);

var dataStoreConfig = builder.Configuration["DataStore"] ?? "InMemory";

#if DEBUG
var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .UseEmulator()
    .AddDatabase("codebreaker");
#else
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");
#endif

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(cosmos)
    .WithEnvironment("DataStore", dataStoreConfig);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.Build().Run();
