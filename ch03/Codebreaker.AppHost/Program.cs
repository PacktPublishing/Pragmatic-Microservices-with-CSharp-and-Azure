var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

// change the DataStore setting in appsettings.json
var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithExternalHttpEndpoints()
    .WithEnvironment("DataStore", dataStore);

if (dataStore == "SqlServer")
{
    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume()
        .AddDatabase("CodebreakerSql", "codebreaker");

    gameApis
        .WithReference(sqlServer);
}
else if (dataStore == "Cosmos")
{
    // When using Cosmos with the following connection string, you need to start the Azure Cosmos emulator on your system, and create a Codebreaker database in this emulator (see information in the readme file)
    var cosmos = builder.AddConnectionString("codebreakercosmos");

    // Comment the previous line and uncomment the next three lines to use the Azure Cosmos emulator in a Docker container (if this is running and doesn't have certificate / timing / HTTPS issues (see the readme file)
    //var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    //    .AddDatabase("codebreaker")
    //    .RunAsEmulator();

    gameApis
        .WithReference(cosmos);
}

builder.Build().Run();
