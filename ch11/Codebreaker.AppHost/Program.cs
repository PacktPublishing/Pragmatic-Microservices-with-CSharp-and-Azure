using Codebreaker.ServiceDefaults;
using Codebreaker.AppHost.Extensions;
using static Codebreaker.ServiceDefaults.ServiceNames;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

CodebreakerSettings settings = new();
builder.Configuration.GetSection("CodebreakerSettings").Bind(settings);

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>(GamesAPIs)
    .WithHttpsHealthCheck("/health")
    .WithEnvironment(EnvVarNames.DataStore, settings.DataStore.ToString())
    .WithEnvironment(EnvVarNames.StartupMode, settings.StartupMode.ToString())
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.CodeBreaker_Bot>(Bot)
    .WithExternalHttpEndpoints()
    .WithReference(gameApis)
    .WaitFor(gameApis);

switch (settings.DataStore)
{
    case DataStoreType.InMemory:
        // no action needed, in-memory is the default
        break;
    case DataStoreType.SqlServer:
        builder.ConfigureSqlServer(gameApis);
        break;
    case DataStoreType.Cosmos:
        builder.ConfigureCosmos(gameApis, settings.UseEmulator);
        break;
    case DataStoreType.Postgres:
        builder.ConfigurePostgres(gameApis);
        break;
    default:
        throw new NotSupportedException($"DataStore {settings.DataStore} is not supported.");
}

builder.Build().Run();
