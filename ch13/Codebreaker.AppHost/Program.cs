
using Codebreaker.AppHost;
using Codebreaker.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

if (builder.Environment.IsPrometheus())
{
    builder.AddUserSecretsForPrometheusEnvironment();
    string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

    var sqlServer = builder.AddSqlServerContainer("sql", sqlPassword)
        .WithVolumeMount("volume.codebreaker.sql", "/var/opt/mssql", VolumeMountType.Named)
        .AddDatabase("CodebreakerSql");

    var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
           .WithVolumeMount("../prometheus", "/etc/prometheus")
           .WithHttpEndpoint(containerPort: 9090, hostPort: 9090);

    var grafana = builder.AddContainer("grafana", "grafana/grafana")
                         .WithVolumeMount("../grafana/config", "/etc/grafana")
                         .WithVolumeMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
                         .WithHttpEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http");

    var redis = builder.AddRedisContainer("redis");

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(sqlServer)
        .WithReference(redis)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"))
        .WithReplicas(1);

    builder.AddProject<Projects.CodeBreaker_Blazor_Host>("blazor")
        .WithReference(gameAPIs);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs);

}
else
{
    var appInsightsConnectionString = builder.Configuration["ApplicationInsightsConnectionString"] ?? throw new InvalidOperationException("Could not read AppInsightsConnectionString");

    var redis = builder.AddRedisContainer("redis");

    //string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");
    //var sqlServer = builder.AddSqlServerContainer("sql", sqlPassword)
    //    .WithVolumeMount("volume.codebreaker.sql", "/var/opt/mssql", VolumeMountType.Named)
    //    .AddDatabase("CodebreakerSql");

//#if DEBUG
//    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
//        .UseEmulator(port: 8081)
//        .AddDatabase("codebreaker")
//        .WithHttpEndpoint(8081, hostPort: 8082, name: "cosmos");
//#else
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");
//#endif

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(cosmos)
        .WithReference(redis)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString)
        .WithReplicas(1);

    builder.AddProject<Projects.CodeBreaker_Blazor_Host>("blazor")
        .WithReference(gameAPIs)
        .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs)
        .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString);
}

builder.Build().Run();
