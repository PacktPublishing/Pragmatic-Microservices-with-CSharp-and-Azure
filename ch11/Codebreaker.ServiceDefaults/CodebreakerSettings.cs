using Microsoft.Extensions.Configuration;

namespace Codebreaker.ServiceDefaults;

public enum DataStoreType
{
    InMemory,
    SqlServer,
    Cosmos,
    Postgres,
    MongoDb
}

public enum TelemetryType
{
    None,
    GrafanaAndPrometheus,
    Seq,
    AzureMonitor,
}

public enum EmulatorOption
{
    PreferDocker,
    PreferLocal,
    UseAzure
}

public class CodebreakerSettings
{
    public DataStoreType DataStore { get; set; } = DataStoreType.InMemory;

    public EmulatorOption UseEmulator { get; set; } = EmulatorOption.PreferDocker;

    public TelemetryType Telemetry { get; set; } = TelemetryType.None;
}

public static class ConfigurationExtensions
{
    public static DataStoreType GetDataStore(this IConfiguration configuration)
    {
        string? dataStore = configuration.GetValue<string>(EnvVarNames.DataStore);
        return Enum.TryParse(dataStore, out DataStoreType result) ? result : DataStoreType.InMemory;
    }
}