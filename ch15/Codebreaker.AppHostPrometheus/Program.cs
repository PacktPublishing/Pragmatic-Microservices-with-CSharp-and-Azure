var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

var sqlServer = builder.AddSqlServer("sql", sqlPassword)
    .WithVolume("volume.codebreaker.sql", "/var/opt/mssql")
    .AddDatabase("CodebreakerSql");

var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
       .WithBindMount("../prometheus", "/etc/prometheus")
       .WithHttpEndpoint(containerPort: 9090, hostPort: 9090);

var grafana = builder.AddContainer("grafana", "grafana/grafana")
                     .WithBindMount("../grafana/config", "/etc/grafana")
                     .WithBindMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
                     .WithHttpEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http");

var redis = builder.AddRedis("redis");

var live = builder.AddProject<Projects.Codebreaker_Live>("live");


var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(live)
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithEnvironment("DataStore", dataStore)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"))
    .WithReplicas(1);

builder.AddProject<Projects.Codebreaker_SqlServerMigration>("sqlmigration")
    .WithReference(sqlServer);

builder.AddProject<Projects.CodeBreaker_Blazor_Host>("blazor")
    .WithReference(gameAPIs);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.Build().Run();
