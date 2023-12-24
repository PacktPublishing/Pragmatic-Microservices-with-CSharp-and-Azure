var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Codebreaker_GameAPIs>("codebreaker.gameapis");

builder.Build().Run();
