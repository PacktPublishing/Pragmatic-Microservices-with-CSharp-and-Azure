var builder = DistributedApplication.CreateBuilder(args);

// change the DataStore setting in Properties/launchSettings.json to either Cosmos or SqlServer
string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithExternalHttpEndpoints()
    .WithEnvironment("DataStore", dataStore);

if (dataStore == "SqlServer")
{
    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume();
    var sqlDB = sqlServer
        .AddDatabase("CodebreakerSql", "codebreaker");

    gameApis
        .WithReference(sqlDB)
        .WaitFor(sqlServer);
}
else if (dataStore == "Cosmos")
{
    // When using Cosmos with the following connection string, you need to start the Azure Cosmos emulator on your system, and create a Codebreaker database in this emulator (see information in the readme file)
    var cosmosDB = builder.AddConnectionString("codebreakercosmos");

    //// Comment the previous line and uncomment the next three lines to use the Azure Cosmos emulator in a Docker container (if this is running and doesn't have certificate / timing / HTTPS issues (see the readme file)
    //var cosmos = builder.AddAzureCosmosDB("codebreakercosmos");
    //var cosmosDB = cosmos
    //    .AddDatabase("codebreaker");

    gameApis
        .WithReference(cosmosDB);
}
else if (dataStore == "Postgres")
{
    var postgres = builder.AddPostgres("postgres")
        .WithPgAdmin()
        .AddDatabase("CodebreakerPostgres");

    gameApis
        .WithReference(postgres);
}

builder.Build().Run();
