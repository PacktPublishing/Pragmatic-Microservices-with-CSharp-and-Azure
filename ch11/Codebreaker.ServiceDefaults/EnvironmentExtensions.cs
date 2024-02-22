using Microsoft.Extensions.Hosting;

namespace Codebreaker.ServiceDefaults;

public static class EnvironmentExtensions
{
    public static bool IsPrometheus(this IHostEnvironment hostEnvironment) =>
        hostEnvironment.IsEnvironment("Prometheus");
}
