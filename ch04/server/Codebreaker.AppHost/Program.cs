var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis");

builder.Build().Run();
