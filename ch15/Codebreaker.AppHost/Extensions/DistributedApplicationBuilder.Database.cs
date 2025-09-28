using Aspire.Hosting.Azure;

namespace Codebreaker.AppHost;

internal static partial class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<IResourceWithConnectionString> AddCosmosConnectionString(this IDistributedApplicationBuilder builder)
    {
        var cosmosdb = builder.AddConnectionString("codebreakercosmos");
        return cosmosdb;
    }

    public static CosmosDBContainerReturn AddCodebreakerCosmos(this IDistributedApplicationBuilder builder, bool useEmulator)
    {
        IResourceBuilder<AzureCosmosDBResource>? cosmos = 
            builder.AddAzureCosmosDB("codebreakercosmos")
                .WithAccessKeyAuthentication();

        if (useEmulator)
        {
#pragma warning disable ASPIRECOSMOSDB001
            // Cosmos emulator running in a Docker container
            // https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux
            cosmos.RunAsPreviewEmulator(p =>
                p.WithDataExplorer()
                    .WithDataVolume("codebreaker-cosmos-data")
                    .WithLifetime(ContainerLifetime.Session));
#pragma warning restore ASPIRECOSDB001
        }

        var cosmosDB = cosmos.AddCosmosDatabase("codebreaker");

        var gamesContainer = cosmosDB.AddContainer("GamesV3", "/PartitionKey");
        var rankingContainer = cosmosDB.AddContainer("RankingsV3", "/PartitionKey");

        return new CosmosDBContainerReturn(cosmos, gamesContainer, rankingContainer);
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddCodebreakerSqlServer(this IDistributedApplicationBuilder builder, bool useEmulator)
    {
        var sqlServer = builder.AddSqlServer("sql")
            .WithDataVolume()
            .AddDatabase("CodebreakerSql");

        return sqlServer;
    }
}

public record class CosmosDBContainerReturn(
    IResourceBuilder<AzureCosmosDBResource> CosmosDB,
    IResourceBuilder<AzureCosmosDBContainerResource> GamesContainer,
    IResourceBuilder<AzureCosmosDBContainerResource> RankingContainer);
