var builder = DistributedApplication.CreateBuilder(args);

var grafana = builder.AddContainer("grafana", "grafana/grafana")
                     .WithVolumeMount("../grafana/config", "/etc/grafana")
                     .WithVolumeMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
                     .WithServiceBinding(containerPort: 3000, hostPort: 3000, name: "grafana-http", scheme: "http");

builder.AddContainer("prometheus", "prom/prometheus")
       .WithVolumeMount("../prometheus", "/etc/prometheus")
       .WithServiceBinding(9090, hostPort: 9090);

var appConfiguration = builder.AddAzureAppConfiguration("CodebreakerAppConfiguration");

string cosmosConnectionString = builder.Configuration["CosmosConnectionString"] ?? throw new InvalidOperationException("Could not find CosmosConnectionString");

var cosmos = builder.AddAzureCosmosDB("GamesCosmosConnection", cosmosConnectionString);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(cosmos)
    .WithReference(appConfiguration)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"));

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs)
    .WithReference(appConfiguration);

builder.Build().Run();
