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
    Valkey
    // Garnet // Garnet currently is not a replacement for Redis: This instance has Lua scripting support disabled, does not work with IDistributedMemory as used from the games API
}

public enum LiveGameMonitoringType
{     
    None,
    SignalR,
    SignalRWithAzure,
}

public class CodebreakerSettings
{
    public DataStoreType DataStore { get; set; } = DataStoreType.InMemory;

    public EmulatorOption UseEmulator { get; set; } = EmulatorOption.PreferDocker;

    public TelemetryType Telemetry { get; set; } = TelemetryType.None;

    public CachingType Caching { get; set; } = CachingType.None;

    public LiveGameMonitoringType LiveGameMonitoring { get; set; } = LiveGameMonitoringType.None;
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

    public static LiveGameMonitoringType GetLiveGameMonitoring(this IConfiguration configuration)
    {
        string? liveGameMonitoring = configuration.GetValue<string>(EnvVarNames.LiveGameMonitoring);
        return Enum.TryParse(liveGameMonitoring, out LiveGameMonitoringType result) ? result : LiveGameMonitoringType.None;
    }
}