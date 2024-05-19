var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("SqlPassword", secret: true);

var sqlServer = builder.AddSqlServer("sql", sqlPassword)
    .WithDataVolume("codebreaker-sql-data", isReadOnly: false)
    .AddDatabase("CodebreakerSql");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(sqlServer);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.Build().Run();
