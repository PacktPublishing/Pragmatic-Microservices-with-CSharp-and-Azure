var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

// var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sqlServer = builder.AddSqlServer("codebreakersql")
    .AddDatabase("codebreaker");

var appConfig = builder.AddAzureAppConfiguration("codebreakerconfig")
    .WithParameter("sku", "Standard");

var keyVault = builder.AddAzureKeyVault("codebreakervault");

builder.AddProject<Projects.ConfigurationPrototype>("configurationprototype")
    .WithReference(appConfig)
    .WithReference(keyVault);

builder.AddProject<Projects.Codebreaker_InitalizeAppConfig>("initappconfig")
    .WithReference(appConfig);

var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    .AddDatabase("codebreaker");

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(sqlServer)
    .WithReference(cosmos)
    .WithReference(appConfig)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(appConfig)
    .WithReference(gameAPIs);

// currently disabled - see https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/issues/81
builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
    .WithReference(cosmos);

builder.AddProject<Projects.Codebreaker_SqlServerMigration>("sqlcreate")
    .WithReference(sqlServer);

builder.Build().Run();
