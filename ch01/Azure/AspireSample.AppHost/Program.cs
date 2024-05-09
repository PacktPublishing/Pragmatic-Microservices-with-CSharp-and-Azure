var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddAzureKeyVault("secrets");

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithReplicas(3)
    .WithReference(keyVault);

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
