using Codebreaker.ServiceDefaults;
using Codebreaker.AppHost.Extensions;
using Microsoft.Extensions.Logging;
using static Codebreaker.ServiceDefaults.ServiceNames;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

CodebreakerSettings settings = new();
builder.Configuration.GetSection("CodebreakerSettings").Bind(settings);

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>(GamesAPIs)
    .WithHttpHealthCheck("/health")
    .WithEnvironment(EnvVarNames.DataStore, settings.DataStore.ToString())
    .WithEnvironment(EnvVarNames.TelemetryMode, settings.Telemetry.ToString())
    .WithExternalHttpEndpoints();

var bot = builder.AddProject<Projects.CodeBreaker_Bot>(Bot)
    .WithExternalHttpEndpoints()
    .WithReference(gameApis)
    .WithEnvironment(EnvVarNames.TelemetryMode, settings.Telemetry.ToString())
    .WaitFor(gameApis);

switch (settings.Telemetry)
{
    case TelemetryType.None:
        // no action needed, just using Aspire dashboard
        break;
    case TelemetryType.GrafanaAndPrometheus:
        var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
           .WithBindMount("../prometheus", "/etc/prometheus", isReadOnly: true)
           .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
           .WithHttpEndpoint(targetPort: 9090, name: "http");

        var grafana = builder.AddContainer("grafana", "grafana/grafana")
            .WithBindMount("../grafana/config", "/etc/grafana", isReadOnly: true)
            .WithBindMount("../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
            .WithHttpEndpoint(targetPort: 3000, name: "http")
            .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"));

        // Get the Aspire Dashboard OTLP endpoint
        var dashboardOtlpUrl = builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] ?? "http://localhost:18889";
        var dashboardOtlpApiKey = builder.Configuration["AppHost:OtlpApiKey"];
        var isHttpsEnabled = dashboardOtlpUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase);

        // Convert localhost to host.docker.internal for container access
        var collectorDashboardUrl = dashboardOtlpUrl.Replace("localhost", "host.docker.internal");

        var otelCollector = builder.AddOpenTelemetryCollector("otelcollector")
            .WithConfig("../otelcollector/config.yaml")
            .WithEndpoint(targetPort: 9464, name: "prometheus") // Expose Prometheus metrics endpoint
            .WithEnvironment("ASPIRE_ENDPOINT", collectorDashboardUrl)
            .WithEnvironment("ASPIRE_API_KEY", dashboardOtlpApiKey ?? string.Empty)
            .WithEnvironment("ASPIRE_INSECURE", isHttpsEnabled ? "false" : "true");

        // Configure services to send telemetry to the OTel Collector
        // Projects run on the host (not in containers), so they need to use localhost
        
        gameApis
            .WithEnvironment(async context =>
            {
                // Projects run on host, use localhost with the proxied port
                var collectorEndpoint = await otelCollector.GetEndpoint("http").GetValueAsync(context.CancellationToken);
                context.Logger.LogInformation("Setting OTEL_EXPORTER_OTLP_ENDPOINT for GameAPIs to: {Url}", collectorEndpoint);
                context.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = collectorEndpoint!;
                // Explicitly set protocol to http/protobuf (default is grpc)
                context.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf";
            })
            .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
            .WaitFor(grafana)
            .WaitFor(otelCollector);
            
        bot
            .WithEnvironment(async context =>
            {
                // Projects run on host, use localhost with the proxied port
                var collectorEndpoint = await otelCollector.GetEndpoint("http").GetValueAsync(context.CancellationToken);
                context.Logger.LogInformation("Setting OTEL_EXPORTER_OTLP_ENDPOINT for Bot to: {Url}", collectorEndpoint);
                Console.WriteLine($"Setting OTEL_EXPORTER_OTLP_ENDPOINT for Bot to: {collectorEndpoint}");
                context.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = collectorEndpoint!;
                // Explicitly set protocol to http/protobuf (default is grpc)
                context.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf";
            })
            .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
            .WaitFor(grafana)
            .WaitFor(otelCollector);
        break;
    case TelemetryType.AzureMonitor:
        // Log Analytics workspace is created automatically
        var appInsights = builder.AddAzureApplicationInsights("insights");
        gameApis.WithReference(appInsights)
            .WaitFor(appInsights);
        bot.WithReference(appInsights)
            .WaitFor(appInsights);
        break;
}

switch (settings.DataStore)
{
    case DataStoreType.InMemory:
        // no action needed, in-memory is the default
        break;
    case DataStoreType.SqlServer:
        builder.ConfigureSqlServer(gameApis);
        break;
    case DataStoreType.Cosmos:
        builder.ConfigureCosmos(gameApis, settings.UseEmulator);
        break;
    case DataStoreType.Postgres:
        builder.ConfigurePostgres(gameApis);
        break;
    default:
        throw new NotSupportedException($"DataStore {settings.DataStore} is not supported.");
}

builder.Build().Run();
