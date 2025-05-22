var builder = DistributedApplication.CreateBuilder(args);

// open appsettings.json and appsettings.Development.json to set the DataStore value
string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string useEmulator = builder.Configuration["UseEmulator"] ?? "PreferDocker";  // options: PreferDocker, PreferLocal, UseAzure

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithHttpsHealthCheck("/health")
    .WithEnvironment("DataStore", dataStore)
    .WithExternalHttpEndpoints();

if (dataStore == "SqlServer")
{
    var sqlDB = builder.AddSqlServer("sql")
        .WithDataVolume("codebreaker-sql-data")
        .AddDatabase("CodebreakerSql", "codebreaker");

    gameApis
        .WithReference(sqlDB)
        .WaitFor(sqlDB);
}
else if (dataStore == "Cosmos")
{
    IResourceBuilder<AzureCosmosDBResource>? cosmos = null;

    if (useEmulator == "PreferLocal")
    {
        // this requires to start the Azure Cosmos DB emulator running on your system
        // running the emulator, create a database named `codebreaker`, a container named `GamesV3` with a partition key `/PartitionKey`!
        // with the other options, this is created automatically with the app-model.

        var cosmosdb = builder.AddConnectionString("codebreakercosmos");

        gameApis
            .WithReference(cosmosdb)
            .WaitFor(cosmosdb);
    }
    else if (useEmulator == "PreferDocker")
    {
        // Cosmos emulator running in a Docker container
        // https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux
        cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
            .RunAsPreviewEmulator(p =>
                p.WithDataExplorer()
                .WithDataVolume("codebreaker-cosmos-data")
                .WithLifetime(ContainerLifetime.Session));
    }
    else
    {
        // Azure Cosmos DB
        cosmos = builder.AddAzureCosmosDB("codebreakercosmos");
    }

    if (useEmulator is not "PreferLocal")
    {
        if (cosmos is null)
        {
            throw new InvalidOperationException("cosmos is null");
        }

        var cosmosDB = cosmos
            .AddCosmosDatabase("codebreaker")
            .AddContainer("GamesV3", "/PartitionKey");

        gameApis
            .WithReference(cosmosDB)
            .WaitFor(cosmosDB);
    }

}
else if (dataStore == "Postgres")
{
    var postgres = builder.AddPostgres("postgres")
        .WithDataVolume("codebreaker-postgres-data")
        .WithPgAdmin(r =>
        {
            r.WithImageTag("latest");
            r.WithImagePullPolicy(ImagePullPolicy.Always);
            r.WithUrlForEndpoint("http", u => u.DisplayText = "PG Admin");
        })
        .AddDatabase("CodebreakerPostgres");

    gameApis
        .WithReference(postgres)
        .WaitFor(postgres);
}

builder.Build().Run();
