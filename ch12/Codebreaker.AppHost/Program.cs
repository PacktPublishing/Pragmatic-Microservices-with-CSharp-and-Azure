using Codebreaker.AppHost;
using Codebreaker.ServiceDefaults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

if (builder.Environment.IsPrometheus())
{
    builder.AddUserSecretsForPrometheusEnvironment();
    string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

    var sqlServer = builder.AddSqlServer("sql", sqlPassword)
        .WithVolume("volume.codebreaker.sql", "/var/opt/mssql", isReadOnly: false)
        .AddDatabase("CodebreakerSql", "codebreaker");

    var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
        .WithBindMount("../prometheus", "/etc/prometheus")
        .WithHttpEndpoint(containerPort: 9090, hostPort: 9090);

    var grafana = builder.AddContainer("grafana", "grafana/grafana")
        .WithBindMount("../grafana/config", "/etc/grafana")
        .WithBindMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
        .WithHttpEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http");

    var redis = builder.AddRedis("redis");

    var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(sqlServer)
        .WithReference(redis)
        .WithEnvironment("DataStore", dataStore)
        .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http"))
        .WithReplicas(1);

    //builder.AddProject<Projects.CodeBreaker_Blazor_Host>("blazor")
    //    .WithReference(gameAPIs);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs);
}
else
{
    var redis = builder.AddRedis("redis");

    IResourceBuilder<ProjectResource>? gameAPIs;

    // The following is an alternative option running locally by using a connection to an existing Azure App Insights resource and a connection to Azure Cosmos DB with the local emulator running (link is configured with appsettings.development.json)
    //string appInsightsConnection = builder.Configuration["ApplicationInsightsConnectionString"] ?? throw new InvalidOperationException("Configure the ApplicationInsightsConnectionString key within user secrets to reference your App Insights Azure resource");
    //string cosmosConnection = builder.Configuration.GetConnectionString("GamesCosmosConnection") ?? throw new InvalidOperationException("Could not read CosmosConnection");
    //gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    //    .WithReference(redis)
    //    .WithEnvironment("APPLICATIONINSIGHTS_CONNECTION_STRING", appInsightsConnection)
    //    .WithEnvironment("DataStore", dataStore)
    //    .WithEnvironment("ConnectionStrings__cosmos", cosmosConnection)
    //    .WithReplicas(1);

    //builder.AddProject<Projects.CodeBreaker_Blazor_Host>("blazor")
    //    .WithReference(gameAPIs)
    //    .WithEnvironment("APPLICATIONINSIGHTS_CONNECTION_STRING", appInsightsConnection);

    //builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    //    .WithReference(gameAPIs)
    //    .WithEnvironment("APPLICATIONINSIGHTS_CONNECTION_STRING", appInsightsConnection);

    var appInsights = builder.AddAzureApplicationInsights("AppInsights");

    var cosmos = builder.AddAzureCosmosDB("cosmos")
        .AddDatabase("codebreaker");

    gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
        .WithReference(cosmos)
        .WithReference(redis)
        .WithReference(appInsights)
        .WithEnvironment("DataStore", dataStore)
        .WithReplicas(1);

    builder.AddProject<Projects.CodeBreaker_Blazor_Host>("blazor")
        .WithReference(gameAPIs)
        .WithReference(appInsights);

    builder.AddProject<Projects.CodeBreaker_Bot>("bot")
        .WithReference(gameAPIs)
        .WithReference(appInsights);
}

builder.Build().Run();
