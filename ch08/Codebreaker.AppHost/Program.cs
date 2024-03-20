var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

builder.AddAzureProvisioning();

//var sqlServer = builder.AddSqlServer("codebreakersql")
//    .PublishAsAzureSqlDatabase()
//    .AddDatabase("codebreaker");

var appConfig = builder.AddAzureAppConfiguration("codebreakerconfig");

var keyVault = builder.AddAzureKeyVault("codebreakervault");

builder.AddProject<Projects.Codebreaker_InitalizeAppConfig>("initappconfig")
    .WithReference(appConfig);

// the name needs to be reduced until this fix: https://github.com/Azure/azure-dev/issues/3496
// don't use cb-cosmos, because of deploy failing with the '-' in the name
var cosmos = builder.AddAzureCosmosDB("cbcosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    // .WithReference(sqlServer)
    .WithReference(cosmos)
    .WithReference(appConfig)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(appConfig)
    .WithReference(gameAPIs);

// currently disabled - see https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/issues/81
//builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
//    .WithReference(cosmos);

//builder.AddProject<Projects.Codebreaker_SqlServerMigration>("sqlcreate")
//    .WithReference(sqlServer);

builder.Build().Run();
