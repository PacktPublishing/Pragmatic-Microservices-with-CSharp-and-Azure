
using Codebreaker.AppHost;
using Codebreaker.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

if (builder.Environment.IsPrometheus())
{
#if DEBUG
    builder.AddUserSecretsForPrometheusEnvironment();
#endif
    string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("Configure SqlPassword");

    var sqlServer = builder.AddSqlServerContainer("sql", sqlPassword)
        .WithVolumeMount("volume.codebreaker.sql", "/var/opt/mssql", VolumeMountType.Named)
        .AddDatabase("CodebreakerSql");

    var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
           .WithVolumeMount("../prometheus", "/etc/prometheus")
           .WithHttpEndpoint(9090, hostPort: 9090);

    var grafana = builder.AddContainer("grafana", "grafana/grafana")
                         .WithVolumeMount("../grafana/config", "/etc/grafana")
                         .WithVolumeMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
                         .WithHttpEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http");

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(sqlServer)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs);
}
else
{
    var appInsightsConnectionString = builder.Configuration["ApplicationInsightsConnectionString"] ?? throw new InvalidOperationException("Could not read AppInsightsConnectionString");

#if DEBUG
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .UseEmulator()
        .AddDatabase("codebreaker");
#else
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");
#endif

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(cosmos)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs)
        .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString);
}

builder.Build().Run();
