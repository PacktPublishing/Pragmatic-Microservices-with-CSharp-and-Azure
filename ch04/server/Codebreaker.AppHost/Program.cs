using Aspire.Hosting;
var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

// change the DataStore setting in appsettings.json
var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithExternalHttpEndpoints()
    .WithEnvironment("DataStore", dataStore);

if (dataStore == "SqlServer")
{
    // set the sql-password with user-secrets within the Parameters category
    var sqlPassword = builder.AddParameter("sql-password", secret: true);

    var sqlServer = builder.AddSqlServer("sql", sqlPassword)
        .AddDatabase("CodebreakerSql", "codebreaker");

    gameApis
        .WithReference(sqlServer);
}
else if (dataStore == "Cosmos")
{
    // When using the connection string from the next line, install the Azure Cosmos emulator on your system, and create a Codebreaker database in this emulator (see readme)
    var cosmos = builder.AddConnectionString("codebreakercosmos");

    // Comment  the previous line and uncomment the next three lines to use the Azure Cosmos emulator in a Docker container (if this is running and doesn't have certificate / timing / HTTPS issues (see readme)
    //var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    //    .AddDatabase("codebreaker")
    //    .RunAsEmulator();

    gameApis
        .WithReference(cosmos);
}

builder.Build().Run();
