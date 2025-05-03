var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposePublisher();

// the password for SQL Server is automatically created on first run and stored in user secrets
// Check user secrets for the stored password
// This password is stored with the Docker volume
// In case you create a new password, delete the Docker volume and run again
// var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sqlServer = builder.AddSqlServer("sql")
    .WithDataVolume("codebreaker-sql-data", isReadOnly: false)
    .AddDatabase("CodebreakerSql");   

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs)
    .WaitFor(gameAPIs);

builder.Build().Run();
