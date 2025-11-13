using Codebreaker.ServiceDefaults;
using Codebreaker.AppHost.Extensions;
using static Codebreaker.ServiceDefaults.ServiceNames;

using Microsoft.Extensions.Configuration;
using MetricsApp.AppHost.OpenTelemetryCollector;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

var builder = DistributedApplication.CreateBuilder(args);

CodebreakerSettings settings = new();
builder.Configuration.GetSection("CodebreakerSettings").Bind(settings);

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>(GamesAPIs)
    .WithHttpHealthCheck("/health")
    .WithEnvironment(EnvVarNames.DataStore, settings.DataStore.ToString())
    .WithEnvironment(EnvVarNames.TelemetryMode, settings.Telemetry.ToString())
    .WithEnvironment(EnvVarNames.Caching, settings.Caching.ToString())
    .WithEnvironment(EnvVarNames.LiveGameMonitoring, settings.LiveGameMonitoring.ToString())
    .WithExternalHttpEndpoints();

var bot = builder.AddProject<Projects.CodeBreaker_Bot>(Bot)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithReference(gameApis)
    .WithEnvironment(EnvVarNames.TelemetryMode, settings.Telemetry.ToString())
    .WaitFor(gameApis);

if (settings.LiveGameMonitoring == LiveGameMonitoringType.SignalR || settings.LiveGameMonitoring == LiveGameMonitoringType.SignalRWithAzure)
{
    var live = builder.AddProject<Projects.Codebreaker_Live>(LiveGameMonitoring)
        .WithHttpHealthCheck("/health")
        .WithExternalHttpEndpoints()
        .WithEnvironment(EnvVarNames.LiveGameMonitoring, settings.LiveGameMonitoring.ToString())
        .WithEnvironment(EnvVarNames.TelemetryMode, settings.Telemetry.ToString());

    var liveClient = builder.AddProject<Projects.Codebreaker_Blazor_LiveClient>("liveclient")
        .WithReference(live)
        .WaitFor(live);

    gameApis.WithReference(live)
        .WaitFor(live);

    if (settings.LiveGameMonitoring == LiveGameMonitoringType.SignalRWithAzure)
    {
        var signalR = builder.AddAzureSignalR("signalr");

        live.WithReference(signalR)
            .WaitFor(signalR);
    }
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
        gameApis.WithReference(appInsights)
            .WaitFor(appInsights);
        bot.WithReference(appInsights)
            .WaitFor(appInsights);
        break;
}

switch (settings.Caching)
{
    case CachingType.None:
        // no action needed, in-memory is the default
        break;
    case CachingType.Redis:
        var redis = builder.AddRedis("redis")
            .WithRedisInsight();
        gameApis.WithReference(redis)
            .WaitFor(redis);
        break;
    case CachingType.Valkey:
        var valkey = builder.AddValkey("valkeycache");
        gameApis.WithReference(valkey)
            .WaitFor(valkey);
        break;
}



builder.Build().Run();
