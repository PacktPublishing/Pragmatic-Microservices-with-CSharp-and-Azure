var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var appConfig = builder.AddAzureAppConfiguration("codebreakerconfig")
    .WithParameter("sku", "Standard");

var keyVault = builder.AddAzureKeyVault("codebreakervault");

builder.AddProject<Projects.ConfigurationPrototype>("configurationprototype")
    .WithReference(appConfig)
    .WithReference(keyVault)
    .WaitFor(appConfig)
    .WaitFor(keyVault);

builder.AddProject<Projects.Codebreaker_InitalizeAppConfig>("initappconfig")
    .WithReference(appConfig)
    .WaitFor(appConfig);

var gameAPIs = builder.AddProject<Projects.Codebreaker_GameAPIs>("gameapis")
    .WithReference(appConfig)
    .WithEnvironment("DataStore", dataStore);

builder.AddProject<Projects.CodeBreaker_Bot>("bot")
    .WithReference(appConfig)
    .WithReference(gameAPIs)
    .WaitFor(gameAPIs);

if (dataStore == "Cosmos")
{
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .AddDatabase("codebreaker");

    var createCosmos = builder.AddProject<Projects.Codebreaker_CosmosCreate>("createcosmos")
        .WithReference(cosmos)
        .WaitFor(cosmos);

    gameAPIs.WithReference(cosmos)
        .WaitForCompletion(createCosmos);
}
else if (dataStore == "SqlServer")
{
    var sqlServer = builder.AddSqlServer("codebreakersql")
        .AddDatabase("codebreaker");

    var createSql = builder.AddProject<Projects.Codebreaker_SqlServerMigration>("sqlcreate")
        .WithReference(sqlServer);

    gameAPIs.WithReference(sqlServer)
        .WaitForCompletion(createSql);
}

builder.Build().Run();
