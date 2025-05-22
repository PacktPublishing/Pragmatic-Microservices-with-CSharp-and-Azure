using Azure;
using Azure.Data.AppConfiguration;

namespace Codebreaker.InitializeAppConfig;

public class AppConfigInitializer(ConfigurationClient configurationClient, IHostApplicationLifetime hostApplicationLifetime, ILogger<AppConfigInitializer> logger) : BackgroundService
{
    private readonly Dictionary<string, string> s_6x4Colors = new()
    {
        { "color1", "Red" },
        { "color2", "Green" },
        { "color3", "Blue" },
        { "color4", "Yellow" },
        { "color5", "Orange" },
        { "color6", "Purple" }
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            foreach ((string key, string color) in s_6x4Colors)
            {
                ConfigurationSetting setting = new($"GameAPIs.Game6x4.{key}", color);
                await configurationClient.AddConfigurationSettingAsync(setting, stoppingToken);
                logger.LogInformation("added setting for key {key}", key);
            }
        }
        catch (RequestFailedException ex) when (ex.HResult == -2146233088)
        {
            logger.LogInformation("Setting was already present");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding settings to App Configuration: {message}", ex.Message);
            throw;
        }
        finally
        {
            logger.LogInformation("Stopping application after adding settings to App Configuration.");
        }
        hostApplicationLifetime.StopApplication();
    }
}
