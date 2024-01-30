using Codebreaker.ServiceDefaults;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Codebreaker.AppHost;

internal static class ApplicationBuilderExtensions
{
    public static void AddUserSecretsForPrometheusEnvironment(this IDistributedApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder.Environment.IsPrometheus())
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
