
var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

builder.AddAzureProvisioning();

//var sqlServer = builder.AddSqlServer("codebreakersql")
//    .PublishAsAzureSqlDatabase()
//    .AddDatabase("codebreaker");

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
//    .WithReference(sqlServer)
    .WithReference(cosmos)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(gameAPIs);

builder.AddProject<Projects.Codebreaker_CosmosCreate>("cosmoscreate")
    .WithReference(cosmos);

//builder.AddProject<Projects.Codebreaker_GameAPIs>("sqlcreate")
//    .WithReference(sqlServer);

builder.Build().Run();
