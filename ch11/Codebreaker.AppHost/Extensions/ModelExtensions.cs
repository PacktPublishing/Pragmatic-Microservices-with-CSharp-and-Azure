using Aspire.Hosting;

using Codebreaker.ServiceDefaults;

using static Codebreaker.ServiceDefaults.ServiceNames;

#pragma warning disable ASPIRECOSMOSDB001

namespace Codebreaker.AppHost.Extensions;
internal static class ModelExtensions
{
    public static void ConfigurePostgres(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> gameApis)
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
    }

    public static void ConfigureSqlServer(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> gameApis)
    {
        var sqlDB = builder.AddSqlServer(SqlResourceName)
            .WithDataVolume(SqlDataVolume)
            .AddDatabase(SqlDatabaseResourceName, SqlDatabaseName);

        gameApis.WithReference(sqlDB)
            .WaitFor(sqlDB);
    }

    public static void ConfigureCosmos(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> gameApis, EmulatorOption emulatorOption)
    {
        IResourceBuilder<AzureCosmosDBResource>? cosmos = null;

        if (emulatorOption == EmulatorOption.PreferLocal)
        {
            // this requires to start the Azure Cosmos DB emulator running on your system
            // running the emulator, create a database named `codebreaker`, a container named `GamesV3` with a partition key `/PartitionKey`!
            // with the other options, this is created automatically with the app-model.

            var cosmosdb = builder.AddConnectionString(CosmosResourceName);

            gameApis
                .WithReference(cosmosdb)
                .WaitFor(cosmosdb);
        }
        else if (emulatorOption == EmulatorOption.PreferDocker)
        {
            // Cosmos emulator running in a Docker container
            // https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux
            cosmos = builder.AddAzureCosmosDB(CosmosResourceName)
                .RunAsPreviewEmulator(p =>
                    p.WithDataExplorer()
                    .WithDataVolume(CosmosDataVolume)
                    .WithLifetime(ContainerLifetime.Session));
        }
        else
        {
            // Azure Cosmos DB
            cosmos = builder.AddAzureCosmosDB(CosmosResourceName);
        }

        if (emulatorOption is not EmulatorOption.PreferLocal)
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
    }

}
