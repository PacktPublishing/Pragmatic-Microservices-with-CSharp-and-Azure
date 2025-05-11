using Microsoft.Extensions.Configuration;

namespace Codebreaker.ServiceDefaults;

public enum DataStoreType
{
    InMemory,
    DistributedMemory,
    SqlServer,
    Cosmos,
    Postgres,
    MongoDb
}

public enum TelemetryType
{
    None,
    GrafanaAndPrometheus,
    AzureMonitor,
}

public enum EmulatorOption
{
    PreferDocker,
    PreferLocal,
    UseAzure
}

public enum CachingType
{
    None,
    Redis,
    // Garnet // Garnet currently is not a replacement for Redis: This instance has Lua scripting support disabled, does not work with IDistributedMemory as used from the games API
}

public class CodebreakerSettings
{
    public DataStoreType DataStore { get; set; } = DataStoreType.InMemory;

    public EmulatorOption UseEmulator { get; set; } = EmulatorOption.PreferDocker;

    public TelemetryType Telemetry { get; set; } = TelemetryType.None;
    public CachingType Caching { get; set; } = CachingType.None;
}

public static class ConfigurationExtensions
{
    public static DataStoreType GetDataStore(this IConfiguration configuration)
    {
        string? dataStore = configuration.GetValue<string>(EnvVarNames.DataStore);
        return Enum.TryParse(dataStore, out DataStoreType result) ? result : DataStoreType.InMemory;
    }

    public static CachingType GetCaching(this IConfiguration configuration)
    {
        string? caching = configuration.GetValue<string>(EnvVarNames.Caching);
        return Enum.TryParse(caching, out CachingType result) ? result : CachingType.None;
    }
}