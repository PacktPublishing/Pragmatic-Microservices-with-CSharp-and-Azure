using Codebreaker.ServiceDefaults;
using static Codebreaker.ServiceDefaults.ServiceNames;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

CodebreakerSettings settings = new();
builder.Configuration.GetSection("CodebreakerSettings").Bind(settings);

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>(GamesAPIs)
    .WithHttpHealthCheck("/health")
    .WithEnvironment(EnvVarNames.DataStore, settings.DataStore.ToString())
    .WithEnvironment(EnvVarNames.StartupMode, settings.StartupMode.ToString())
    .WithExternalHttpEndpoints();

var ConfigureSqlServer = () => {
    var sqlDB = builder.AddSqlServer(SqlResourceName)
    .WithDataVolume(SqlDataVolume)
    .AddDatabase(SqlDatabaseResourceName, SqlDatabaseName);

    gameApis
        .WithReference(sqlDB)
        .WaitFor(sqlDB);
};

var ConfigureCosmos = () =>
{
    IResourceBuilder<AzureCosmosDBResource>? cosmos = null;

    if (settings.UseEmulator == EmulatorOption.PreferLocal)
    {
        // this requires to start the Azure Cosmos DB emulator running on your system
        // running the emulator, create a database named `codebreaker`, a container named `GamesV3` with a partition key `/PartitionKey`!
        // with the other options, this is created automatically with the app-model.

        var cosmosdb = builder.AddConnectionString(CosmosResourceName);

        gameApis
            .WithReference(cosmosdb)
            .WaitFor(cosmosdb);
    }
    else if (settings.UseEmulator == EmulatorOption.PreferDocker)
    {
#pragma warning disable ASPIRECOSMOSDB001
        // Cosmos emulator running in a Docker container
        // https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux
        cosmos = builder.AddAzureCosmosDB(CosmosResourceName)
            .RunAsPreviewEmulator(p =>
                p.WithDataExplorer()
                .WithDataVolume(CosmosDataVolume)
                .WithLifetime(ContainerLifetime.Session));
#pragma warning restore ASPIRECOSMOSDB001
    }
    else
    {
        // Azure Cosmos DB
        cosmos = builder.AddAzureCosmosDB(CosmosResourceName);
    }

    if (settings.UseEmulator is not EmulatorOption.PreferLocal)
    {
        if (cosmos is null)
        {
            throw new InvalidOperationException("cosmos is null");
        }

        var cosmosDB = cosmos
            .AddCosmosDatabase(CosmosDatabaseName)
            .AddContainer(CosmosContainerName, CosmosPartitionKey);

        gameApis
            .WithReference(cosmosDB)
            .WaitFor(cosmosDB);
    }
};

var ConfigurePostgres = () =>
{
    var postgres = builder.AddPostgres(PostgresResourceName)
    .WithDataVolume(PostgresDataVolume)
    .WithPgAdmin(r =>
    {
        r.WithImageTag("latest");
        r.WithImagePullPolicy(ImagePullPolicy.Always);
        r.WithUrlForEndpoint("http", u => u.DisplayText = "PG Admin");
    })
    .AddDatabase(PostgresDatabaseName);

    gameApis
        .WithReference(postgres)
        .WaitFor(postgres);
};

builder.AddProject<Projects.CodeBreaker_Bot>(Bot)
    .WithExternalHttpEndpoints()
    .WithReference(gameApis)
    .WaitFor(gameApis);

switch (settings.DataStore)
{
    case DataStoreType.InMemory:
        // no action needed, in-memory is the default
        break;
    case DataStoreType.SqlServer:
        ConfigureSqlServer();
        break;
    case DataStoreType.Cosmos:
        ConfigureCosmos();
        break;
    case DataStoreType.Postgres:
        ConfigurePostgres();
        break;
    default:
        throw new NotSupportedException($"DataStore {settings.DataStore} is not supported.");
}

builder.Build().Run();
