using Azure.Core;

namespace Codebreaker.GameAPIs.Configuration;

public static class AzureAppConfigurationExtensions
{
    public static void AddAndConfigureAzureAppConfiguration(this IConfigurationBuilder builder, Uri endpoint, TokenCredential credential)
    {
        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(endpoint, credential);  
        });
    }
}
