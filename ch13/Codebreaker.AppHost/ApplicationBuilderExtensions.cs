using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Codebreaker.AppHost;

internal static class ApplicationBuilderExtensions
{
    [Obsolete("Is this used somewhere?", error: true)]
    public static void AddUserSecretsForPrometheusEnvironment(this IDistributedApplicationBuilder applicationBuilder)
    {
        if (Environment.GetEnvironmentVariable("StartupMode") == "OnPremises")
        {
            string appName = applicationBuilder.Environment.ApplicationName;
            if (!string.IsNullOrEmpty(appName))
            {
                var appAssembly = Assembly.Load(new AssemblyName(appName));
                applicationBuilder.Configuration.AddUserSecrets(appAssembly, optional: true);
            }
        }
    }
}
