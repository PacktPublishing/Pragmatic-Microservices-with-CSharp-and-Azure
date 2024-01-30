using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Codebreaker.ServiceDefaults;

public static class EnvironmentExtensions
{
    public static bool IsPrometheus(this IHostEnvironment hostEnvironment) =>
        hostEnvironment.IsEnvironment("Prometheus");
}
