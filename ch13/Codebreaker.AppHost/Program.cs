

var builder = DistributedApplication.CreateBuilder(args);

//var startupMode = builder.AddParameter("StartupMode");


string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

//if (builder.Environment.IsPrometheus())
//{
//    var sqlPasswordResource = builder.AddParameter("sqlPassword", secret: true);

////    string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

//    var sqlServer = builder.AddSqlServer("sql", password: sqlPasswordResource)
//        .WithVolume("volume.codebreaker.sql", "/var/opt/mssql")
//        .AddDatabase("CodebreakerSql");

//    var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
//           .WithVolume("../prometheus", "/etc/prometheus")
//           .WithHttpEndpoint(containerPort: 9090, hostPort: 9090);

//    var grafana = builder.AddContainer("grafana", "grafana/grafana")
//                         .WithVolume("../grafana/config", "/etc/grafana")
//                         .WithVolume("../grafana/dashboards", "/var/lib/grafana/dashboards")
//                         .WithHttpEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http");

//    var redis = builder.AddRedis("redis")
//        .PublishAsContainer();

//    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
//        .WithReference(sqlServer)
//        .WithReference(redis)
//        .WithEnvironment("DataStore", dataStore)
//        .WithEnvironment("")
//        .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"))
//        .WithReplicas(1);

//    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
//        .WithReference(gameAPIs);

//}
//else

builder.AddAzureProvisioning();

var logs = builder.AddAzureLogAnalyticsWorkspace("logs");
var insights = builder.AddAzureApplicationInsights("insights", logs);

var redis = builder.AddRedis("redis")
    .PublishAsContainer();

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

//var live = builder.AddProject<Projects.Codebreaker_Live>("live")
//    .WithReference(insights)
//    .WithReplicas(1);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(cosmos)
    .WithReference(redis)
    .WithReference(insights)
    // .WithReference(live)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(insights)
    .WithReference(gameAPIs);



builder.Build().Run();
