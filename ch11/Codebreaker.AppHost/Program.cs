var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("prometheus", "prom/prometheus")
       .WithVolumeMount("../prometheus", "/etc/prometheus")
       .WithServiceBinding(9090, hostPort: 9090);

var appConfiguration = builder.AddAzureAppConfiguration("CodebreakerAppConfiguration");

string cosmosConnectionString = builder.Configuration["CosmosConnectionString"] ?? throw new InvalidOperationException("Could not find CosmosConnectionString");

var cosmos = builder.AddAzureCosmosDB("GamesCosmosConnection", cosmosConnectionString);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(cosmos)
    .WithReference(appConfiguration);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs)
    .WithReference(appConfiguration);

builder.Build().Run();
