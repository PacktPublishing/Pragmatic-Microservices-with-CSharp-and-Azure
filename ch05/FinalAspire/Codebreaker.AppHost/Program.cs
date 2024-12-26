var builder = DistributedApplication.CreateBuilder(args);

// the parameter with the name sql-password is automatically retrieved from secrets if no password is supplied with the AddSqlServer method
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
