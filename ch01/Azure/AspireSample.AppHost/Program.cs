var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddAzureKeyVault("secrets");

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithReference(keyVault)
    .WithHttpsHealthCheck("/health")
    .WaitFor(keyVault)

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
