
var builder = DistributedApplication.CreateBuilder(args);

#if PROMETHEUS

string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

var sqlServer = builder.AddSqlServerContainer("sql", sqlPassword)
    .WithVolumeMount("volume.codebreaker.sql", "/var/opt/mssql", VolumeMountType.Named)
    .AddDatabase("CodebreakerSql");

var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
       .WithVolumeMount("../prometheus", "/etc/prometheus")
       .WithServiceBinding(9090, hostPort: 9090);

var grafana = builder.AddContainer("grafana", "grafana/grafana")
                     .WithVolumeMount("../grafana/config", "/etc/grafana")
                     .WithVolumeMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
                     .WithServiceBinding(containerPort: 3000, hostPort: 3000, name: "grafana-http", scheme: "http");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(sqlServer)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));

var bot = builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

#else

var appInsightsConnectionString = builder.Configuration["ApplicationInsightsConnectionString"] ?? throw new InvalidOperationException("Could not read AppInsightsConnectionString");

var appConfiguration = builder.AddAzureAppConfiguration("CodebreakerAppConfiguration");
string cosmosConnectionString = builder.Configuration["CosmosConnectionString"] ?? throw new InvalidOperationException("Could not find CosmosConnectionString");

var cosmos = builder.AddAzureCosmosDB("GamesCosmosConnection", cosmosConnectionString);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(cosmos)
    .WithReference(appConfiguration)
    .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString);

var bot = builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs)
    .WithReference(appConfiguration)
    .WithEnvironment("ApplicationInsightsConnectionString", appInsightsConnectionString);

#endif

builder.Build().Run();
