using Codebreaker.ServiceDefaults;
using Codebreaker.AppHost.Extensions;
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
        var prometheus = builder.AddPrometheus("prometheus");

        var grafana = builder.AddGrafana("grafana")
            .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"));

        // Get the Aspire Dashboard OTLP endpoint
        var dashboardOtlpUrl = builder.Configuration["ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL"] ?? "http://localhost:18889";
        var dashboardOtlpApiKey = builder.Configuration["AppHost:OtlpApiKey"];
        var isHttpsEnabled = dashboardOtlpUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase);

        builder.AddOpenTelemetryCollector("otelcollector")
            .WithConfig("../otelcollector/config.yaml")
            .WithEndpoint(targetPort: 9464, name: "prometheus") // Expose Prometheus metrics endpoint
            .WithEnvironment("ASPIRE_ENDPOINT", dashboardOtlpUrl)
            .WithEnvironment("ASPIRE_API_KEY", dashboardOtlpApiKey ?? string.Empty)
            .WithEnvironment("ASPIRE_INSECURE", isHttpsEnabled ? "false" : "true");

        gameApis.WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
            .WaitFor(grafana);
        bot.WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
            .WaitFor(grafana);
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
