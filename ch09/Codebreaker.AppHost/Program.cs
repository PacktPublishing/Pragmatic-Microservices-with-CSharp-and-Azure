var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

string startupMode = Environment.GetEnvironmentVariable("STARTUP_MODE") ?? "Azure";
bool useAzureADB2C = startupMode == "Azure";

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
  .WithReference(cosmos)
  .WithEnvironment("DataStore", dataStore);

var bot = builder.AddProject<Projects.CodeBreaker_Bot>("bot")
  .WithReference(gameAPIs);

if (startupMode == "OnPremises")
{
    var usersDbName = "usersdb";
    var mySqlPassword = builder.AddParameter("mysql-password", secret: true);

    var usersDb = builder.AddMySql("mysql", password: mySqlPassword)
        .WithEnvironment("MYSQL_DATABASE", usersDbName)
        .WithDataVolume()
        .WithPhpMyAdmin()
        .AddDatabase(usersDbName);

    var gateway = builder.AddProject<Projects.Codebreaker_ApiGateway_Identities>("gateway-identities")
        .WithReference(gameAPIs)
        .WithReference(bot)
        .WithReference(usersDb)
        .WithExternalHttpEndpoints();

    builder.AddProject<Projects.WebAppAuth>("webapp")
        .WithReference(gateway)
        .WithEnvironment("USEAADB2C", useAzureADB2C.ToString());
}
else
{
    var gateway = builder.AddProject<Projects.Codebreaker_ApiGateway>("gateway")
        .WithReference(gameAPIs)
        .WithReference(bot)
        .WithExternalHttpEndpoints();

    builder.AddProject<Projects.WebAppAuth>("webapp")
        .WithReference(gateway)
        .WithEnvironment("USEAADB2C", useAzureADB2C.ToString());
}

builder.Build().Run();
