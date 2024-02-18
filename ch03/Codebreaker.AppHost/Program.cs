using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var sqlServerConnectionString = builder.Configuration.GetConnectionString("GamesSqlServerConnection") ?? throw new InvalidOperationException("Could not read GamesSqlServerConnection");
var cosmosConnectionString = builder.Configuration.GetConnectionString("GamesCosmosConnection") ?? throw new InvalidOperationException("Could not read GamesCosmosConnection");

builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithEnvironment("DataStore", dataStore)
    .WithEnvironment("ConnectionStrings__GamesSqlServerConnection", sqlServerConnectionString)
    .WithEnvironment("ConnectionStrings__GamesCosmosConnection", cosmosConnectionString);

builder.Build().Run();
