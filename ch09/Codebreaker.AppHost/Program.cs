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

    var identityEndpoint = gateway.GetEndpoint("Https");

    var webApp = builder.AddProject<Projects.WebAppAuth>("webapp")
        .WithReference(gateway)
        .WithEnvironment("USEAADB2C", useAzureADB2C.ToString())
        .WithEnvironment("Identity__Url", identityEndpoint);

    // Wire up the callback urls (self referencing)
    webApp.WithEnvironment("CallBackUrl", webApp.GetEndpoint("Https"));
}
else
{
    var gateway = builder.AddProject<Projects.Codebreaker_ApiGateway>("gateway")
        .WithReference(gameAPIs)
        .WithReference(bot)
        .WithExternalHttpEndpoints();

    var webApp = builder.AddProject<Projects.WebAppAuth>("webapp")
        .WithReference(gateway)
        .WithEnvironment("USEAADB2C", useAzureADB2C.ToString());
}

builder.Build().Run();
