var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposePublisher();

// the password for SQL Server is automatically created on first run and stored in user secrets
// Check user secrets for the stored password
// This password is stored with the Docker volume
// In case you create a new password, delete the Docker volume and run again
// var sqlPassword = builder.AddParameter("sql-password", secret: true);

// step 1 done: configure a SQL Server container with a named volume
var sqlServer = builder.AddSqlServer("sql")
    .WithDataVolume("codebreaker-sql-data", isReadOnly: false)
    .AddDatabase("CodebreakerSql");   

// step 2 done: configure the Game APIs project using the SQL Server container
var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

// step 4 done: configure the Bot project using the Game APIs project
builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs)
    .WaitFor(gameAPIs);

builder.Build().Run();
