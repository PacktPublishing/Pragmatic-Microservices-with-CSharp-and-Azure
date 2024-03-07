using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

var sqlServer = builder.AddSqlServer("sql", sqlPassword)
    .AddDatabase("CodebreakerSql", "codebreaker");

// TODO: currently having issues with the Docker container for the Cosmos DB emulator
// Use the local installed emulator for now

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

if (builder.Environment.IsDevelopment())
{
    string cosmosConnection = builder.Configuration.GetConnectionString("GamesCosmosConnection") ?? throw new InvalidOperationException("Could not read CosmosConnection");

    builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithEnvironment("DataStore", dataStore)
        .WithReference(sqlServer)
        .WithEnvironment("ConnectionStrings__codebreakercosmos", cosmosConnection);
}
else
{
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker")
        .RunAsEmulator();

    builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithEnvironment("DataStore", dataStore)
        .WithReference(cosmos)
        .WithReference(sqlServer);
}

builder.Build().Run();
