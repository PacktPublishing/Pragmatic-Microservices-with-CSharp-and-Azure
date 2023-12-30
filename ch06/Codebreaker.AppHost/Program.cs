var builder = DistributedApplication.CreateBuilder(args);

string cosmosConnectionString = builder.Configuration["CosmosConnectionString"] ?? throw new InvalidOperationException("Could not find CosmosConnectionString");

var cosmos = builder.AddAzureCosmosDB("GamesCosmosConnection", cosmosConnectionString);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    //.WithReference(sqlServer)
    .WithReference(cosmos);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.Build().Run();
