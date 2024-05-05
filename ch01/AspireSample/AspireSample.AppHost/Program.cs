var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice");

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
