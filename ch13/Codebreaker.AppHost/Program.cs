
using Codebreaker.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";
string startupMode = builder.Configuration["STARTUP_MODE"] ?? "Azure";  // specified with environment variables in the launch profile

var redis = builder.AddRedis("redis")
    .WithRedisCommander()
    .PublishAsContainer();

if (startupMode == "OnPremises")
{
    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume()
        .PublishAsContainer()
        .AddDatabase("CodebreakerSql");

    var grafana = builder.AddContainer("grafana", "grafana/grafana")
        .WithBindMount("../grafana/config", "/etc/grafana", isReadOnly: true)
        .WithBindMount("../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
        .WithHttpEndpoint(targetPort: 3000, name: "grafana-http");

    builder.AddUserSecretsForPrometheusEnvironment();

    var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
       .WithBindMount("../prometheus", "/etc/prometheus", isReadOnly: true)
       .WithHttpEndpoint(/* This port is fixed as it's referenced from the Grafana config */ port: 9090, targetPort: 9090);

    var live = builder.AddProject<Projects.Codebreaker_Live>("live")
        .WithExternalHttpEndpoints()
        .WithEnvironment("StartupMode", startupMode);

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(sqlServer)
        .WithReference(redis)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"))
        .WithEnvironment("StartupMode", startupMode);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs)
        .WithEnvironment("StartupMode", startupMode);
}
else
{
    var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
    var appInsights = builder.AddAzureApplicationInsights("insights", logs);
    var signalR = builder.AddAzureSignalR("signalr");

    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");

    var live = builder.AddProject<Projects.Codebreaker_Live>("live")
        .WithExternalHttpEndpoints()
        .WithReference(appInsights)
        .WithReference(signalR)
        .WithEnvironment("StartupMode", startupMode);

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithExternalHttpEndpoints()
        .WithReference(cosmos)
        .WithReference(redis)
        .WithReference(appInsights)
        .WithReference(live)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("StartupMode", startupMode);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithExternalHttpEndpoints()
        .WithReference(gameAPIs)
        .WithReference(appInsights)
        .WithEnvironment("StartupMode", startupMode);
}

builder.Build().Run();