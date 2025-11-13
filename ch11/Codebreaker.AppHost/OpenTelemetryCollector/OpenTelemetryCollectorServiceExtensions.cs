using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MetricsApp.AppHost.OpenTelemetryCollector;

internal static class OpenTelemetryCollectorServiceExtensions
{
    private const string OtelExporterOtlpEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";

    public static IDistributedApplicationBuilder AddOpenTelemetryCollectorInfrastructure(this IDistributedApplicationBuilder builder)
    {
        builder.Eventing.Subscribe<ResourceEndpointsAllocatedEvent>(async (@event, cancellationToken) =>
        {
            // Only process when the OpenTelemetry Collector resource gets its endpoints allocated
            if (@event.Resource is not OpenTelemetryCollectorResource collectorResource)
            {
                return;
            }

            var logger = @event.Services.GetRequiredService<ILogger<OpenTelemetryCollectorResource>>();
            var appModel = @event.Services.GetRequiredService<DistributedApplicationModel>();

            var endpoint = collectorResource.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName);
            if (!endpoint.Exists)
            {
                logger.LogWarning($"No {OpenTelemetryCollectorResource.OtlpGrpcEndpointName} endpoint for the collector.");
                return;
            }

            // Apply environment variable to all resources in the application model
            foreach (var resource in appModel.Resources)
            {
                resource.Annotations.Add(new EnvironmentCallbackAnnotation((EnvironmentCallbackContext context) =>
                {
                    if (context.EnvironmentVariables.ContainsKey(OtelExporterOtlpEndpoint))
                    {
                        logger.LogDebug("Forwarding telemetry for {ResourceName} to the collector.", resource.Name);

                        context.EnvironmentVariables[OtelExporterOtlpEndpoint] = endpoint;
                    }
                }));
            }

            await Task.CompletedTask;
        });

        return builder;
    }
}
