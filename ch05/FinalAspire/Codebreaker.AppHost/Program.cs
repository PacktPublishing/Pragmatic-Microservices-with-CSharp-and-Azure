var builder = DistributedApplication.CreateBuilder(args);

string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

var sqlServer = builder.AddSqlServerContainer("sql", sqlPassword)
    .WithVolumeMount("volume.codebreaker.sql", "/var/opt/mssql", VolumeMountType.Named)
    .AddDatabase("CodebreakerSql");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(sqlServer);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.Build().Run();
