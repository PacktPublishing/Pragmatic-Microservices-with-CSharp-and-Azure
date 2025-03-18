var builder = DistributedApplication.CreateBuilder(args);

// open appsettings.json and appsettings.Development.json to set the DataStore value
string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string useEmulator = builder.Configuration["UseEmulator"] ?? "PreferDocker";  // options: PreferDocker, PreferLocal, UseAzure

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
        // Cosmos emulator running in a container
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
        cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
            .WithAccessKeyAuthentication();
    }

    if (useEmulator is not "PreferLocal")
    {
        if (cosmos is null) throw new InvalidOperationException("cosmos is null");

        var cosmosDB = cosmos
            .AddCosmosDatabase("codebreaker")
            .AddContainer("GamesV3", "/PartitionKey");

        gameApis
            .WithReference(cosmos)
            .WaitFor(cosmos)
            .WithEnvironment(context =>
            {
                if (cosmos.Resource.IsEmulator || cosmos.Resource.UseAccessKeyAuthentication)
                {
                    context.EnvironmentVariables["Aspire__Microsoft__EntityFrameworkCore__Cosmos__CodebreakerCosmos__ConnectionString"] = cosmos.Resource.ConnectionStringExpression;
                }
                else
                {
                    context.EnvironmentVariables["Aspire__Microsoft__EntityFrameworkCore__Cosmos__CodebreakerCosmos__AccountEndpoint"] = cosmos.Resource.ConnectionStringExpression;
                }
            });
        // environment temporary workaround until 9.2  https://github.com/dotnet/aspire/issues/7785#issuecomment-2686122073
    }

}
else if (dataStore == "Postgres")
{
    var postgres = builder.AddPostgres("postgres")
        .WithPgAdmin()
        .AddDatabase("CodebreakerPostgres");

    gameApis
        .WithReference(postgres)
        .WaitFor(postgres);
}
else if (dataStore == "Mongo")
{
    var mongo = builder.AddMongoDB("mongo")
        .WithMongoExpress()
        .AddDatabase("CodebreakerMongo");

    gameApis
        .WithReference(mongo)
        .WaitFor(mongo);
}

builder.Build().Run();
