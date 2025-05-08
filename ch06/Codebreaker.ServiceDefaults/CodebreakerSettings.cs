namespace Codebreaker.ServiceDefaults;

public enum DataStoreType
{
    InMemory,
    SqlServer,
    Cosmos,
    Postgres,
    MongoDb,
}

public enum EmulatorOption
{
    PreferDocker,
    PreferLocal,
    UseAzure
}

public class CodebreakerSettings
{
    // public string DataStore { get; set; } = "InMemory";
    public DataStoreType DataStore { get; set; } = DataStoreType.InMemory; // options: InMemory, SqlServer, CosmosDb, MongoDb, Redis
    public EmulatorOption UseEmulator { get; set; } = EmulatorOption.PreferDocker;  // options: PreferDocker, PreferLocal, UseAzure
}
