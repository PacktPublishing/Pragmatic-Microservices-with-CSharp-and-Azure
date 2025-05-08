using Codebreaker.ServiceDefaults;
using static Codebreaker.ServiceDefaults.ServiceNames;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIREAZURE001
#pragma warning disable ASPIREPUBLISHERS001
#pragma warning disable ASPIRECOSMOSDB001

if (builder.ExecutionContext.PublisherName == "azure" ||
    builder.ExecutionContext.IsInspectMode)
{
    builder.AddAzurePublisher();
}

if (builder.ExecutionContext.PublisherName == "docker-compose" ||
    builder.ExecutionContext.IsInspectMode)
{
    builder.AddDockerComposePublisher();
}

if (builder.ExecutionContext.PublisherName == "kubernetes" ||
    builder.ExecutionContext.IsInspectMode)
{
    builder.AddKubernetesPublisher("k8s-envr");
}

// open appsettings.json and appsettings.Development.json to set the DataStore value

CodebreakerSettings settings = new();
builder.Configuration.GetSection("CodebreakerSettings").Bind(settings);

//string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
//string useEmulator = builder.Configuration["UseEmulator"] ?? "PreferDocker";  // options: PreferDocker, PreferLocal, UseAzure

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>(GamesAPIs)
    .WithHttpsHealthCheck("/health")
    .WithEnvironment("DataStore", settings.DataStore.ToString())
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.CodeBreaker_Bot>(Bot)
    .WithExternalHttpEndpoints()
    .WithReference(gameApis)
    .WaitFor(gameApis);

var ConfigureSqlServer = () => {
    var sqlDB = builder.AddSqlServer("sql")
    .WithDataVolume("codebreaker-sql-data")
    .AddDatabase("CodebreakerSql", "codebreaker");

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

        var cosmosdb = builder.AddConnectionString("codebreakercosmos");

        gameApis
            .WithReference(cosmosdb)
            .WaitFor(cosmosdb);
    }
    else if (settings.UseEmulator == EmulatorOption.PreferDocker)
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

    if (settings.UseEmulator is not EmulatorOption.PreferLocal)
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
};

var ConfigurePostres = () =>
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
};

switch (settings.DataStore)
{
    case DataStoreType.InMemory:
        // no action needed, in-memory is default
        break;
    case DataStoreType.SqlServer:
        ConfigureSqlServer();
        break;
    case DataStoreType.Cosmos:
        ConfigureCosmos();
        break;
    case DataStoreType.Postgres:
        ConfigurePostres();
        break;
    default:
        throw new NotSupportedException($"DataStore {settings.DataStore} is not supported.");

}

builder.Build().Run();

#pragma warning restore ASPIREAZURE001
#pragma warning restore ASPIREPUBLISHERS001
#pragma warning restore ASPIRECOSMOSDB001