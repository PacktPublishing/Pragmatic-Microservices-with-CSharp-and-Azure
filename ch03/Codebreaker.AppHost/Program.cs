var builder = DistributedApplication.CreateBuilder(args);

string sqlPassword = builder.Configuration["SqlPassword"] ?? throw new InvalidOperationException("could not read password");

var sqlServer = builder.AddSqlServer("sql", sqlPassword)
    .PublishAsContainer<SqlServerServerResource>()
    .WithVolume("volume.codebreaker.sql", "/var/opt/mssql", isReadOnly: false)
    .AddDatabase("CodebreakerSql");

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker")
    .RunAsEmulator();

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

// Use the following code to set the connection strings for the SQL Server and Cosmos DB databases
// var sqlServerConnectionString = builder.Configuration.GetConnectionString("CodebreakerSql") ?? throw new InvalidOperationException("Could not read SQL Server connection");
// var cosmosConnectionString = builder.Configuration.GetConnectionString("codebreaker") ?? throw new InvalidOperationException("Could not read Azure Cosmos DB connection");

builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithEnvironment("DataStore", dataStore)
    .WithReference(cosmos)
    .WithReference(sqlServer);
//    .WithEnvironment("ConnectionStrings__GamesSqlServerConnection", sqlServerConnectionString)
//    .WithEnvironment("ConnectionStrings__GamesCosmosConnection", cosmosConnectionString);

builder.Build().Run();
