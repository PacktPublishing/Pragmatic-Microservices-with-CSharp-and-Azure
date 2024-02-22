using Microsoft.Extensions.Hosting;

namespace Codebreaker.ServiceDefaults;
// Preview 3 - fast fix, move this to a library outside of ServiceDefaults
// see https://github.com/dotnet/aspire-samples/pull/108
public static class EnvironmentExtensions
{
    public static bool IsPrometheus(this IHostEnvironment hostEnvironment) =>
        hostEnvironment.IsEnvironment("Prometheus");
}
