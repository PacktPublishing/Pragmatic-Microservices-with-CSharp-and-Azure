using Codebreaker.ServiceDefaults;
using Codebreaker.AppHost.Extensions;
using static Codebreaker.ServiceDefaults.ServiceNames;

using Microsoft.Extensions.Configuration;
using MetricsApp.AppHost.OpenTelemetryCollector;

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
        // no action needed, just using .NET Aspire dashboard
        break;
    case TelemetryType.GrafanaAndPrometheus:
        var prometheus = builder.AddPrometheus("prometheus");

        var grafana = builder.AddGrafana("grafana")
            .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"));

        builder.AddOpenTelemetryCollector("otelcollector", "../otelcollector/config.yaml")
          .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp");

        gameApis.WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
            .WaitFor(grafana);
        bot.WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"))
            .WaitFor(grafana);
        break;
    case TelemetryType.AzureMonitor:
        // var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
        // var appInsights = builder.AddAzureApplicationInsights("insights", logs);
        // gameApis.WithEnvironment("LOG_ANALYTICS_WORKSPACE_ID", $"{laws.WorkspaceId}");

        // Log Analytics workspace is created automatically
        var appInsights = builder.AddAzureApplicationInsights("insights");
        gameApis.WithReference(appInsights).WaitFor(appInsights);
        bot.WithReference(appInsights).WaitFor(appInsights);
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
