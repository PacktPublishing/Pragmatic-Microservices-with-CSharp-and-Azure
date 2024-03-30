using Azure.Data.AppConfiguration;

namespace Codebreaker.InitalizeAppConfig;

public class AppConfigInitializer(ConfigurationClient configurationClient, IHostApplicationLifetime hostApplicationLifetime, ILogger<AppConfigInitializer> logger) : BackgroundService
{
    private Dictionary<string, string> s_6x4Colors = new()
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
        foreach ((string key, string color) in s_6x4Colors)
        {
            ConfigurationSetting setting = new($"GameAPIs.Game6x4.{key}", color);
            await configurationClient.AddConfigurationSettingAsync(setting);
            logger.LogInformation("added setting for key {key}", key);
        }
        hostApplicationLifetime.StopApplication();
    }
}
