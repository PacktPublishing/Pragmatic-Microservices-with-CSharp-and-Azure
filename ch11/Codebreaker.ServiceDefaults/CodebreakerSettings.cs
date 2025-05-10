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
