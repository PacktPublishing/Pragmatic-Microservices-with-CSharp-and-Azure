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

public enum EmulatorOption
{
    PreferDocker,
    PreferLocal,
    UseAzure
}

public enum StartupMode
{
    LocalDev,
    Azure,
    OnPremises
}

public class CodebreakerSettings
{
    public DataStoreType DataStore { get; set; } = DataStoreType.InMemory;

    public EmulatorOption UseEmulator { get; set; } = EmulatorOption.PreferDocker;

    public StartupMode StartupMode { get; set; } = StartupMode.LocalDev;
}

public static class ConfigurationExtensions
{
    /// <summary>
    /// Retrieves the data store type specified in the configuration.
    /// </summary>
    /// <param name="configuration">The configuration instance to retrieve the data store type from.</param>
    /// <returns>The <see cref="DataStoreType"/> value parsed from the configuration.  If the configuration value is not set or
    /// cannot be parsed, returns <see cref="DataStoreType.InMemory"/>.</returns>
    public static DataStoreType GetDataStoreType(this IConfiguration configuration)
    {         
        string? dataStore = configuration.GetValue<string>(EnvVarNames.DataStore);
        if (Enum.TryParse<DataStoreType>(dataStore, out var dataStoreType))
        {
             return dataStoreType;
        }
        else
        {
            return DataStoreType.InMemory;
        }
    }
}
