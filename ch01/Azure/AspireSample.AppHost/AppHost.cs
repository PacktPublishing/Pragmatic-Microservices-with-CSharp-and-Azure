var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddAzureKeyVault("secrets");

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithReference(keyVault)
    .WithHttpHealthCheck("/health")
    .WaitFor(keyVault);

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
