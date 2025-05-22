namespace Codebreaker.ServiceDefaults;

public class ServiceNames
{
    public const string GamesAPIs = "gameapis";
    public const string Bot = "bot";

    public const string CosmosResourceName = "codebreakercosmos";
    public const string CosmosDataVolume = "codebreaker-cosmos-data";
    public const string CosmosDatabaseName = "codebreaker";
    public const string CosmosContainerName = "GamesV3";
    public const string CosmosPartitionKey = @"/PartitionKey";

    public const string SqlResourceName = "sql";
    public const string SqlDataVolume = "codebreaker-sql-data";
    public const string SqlDatabaseResourceName = "codebreakersql";
    public const string SqlDatabaseName = "codebreaker";

    public const string PostgresResourceName = "codebreakerpostgres";
    public const string PostgresDataVolume = "codebreaker-postgres-data";
    public const string PostgresDatabaseName = "codebreaker";

    public const string LiveGameMonitoring = "live";
}
