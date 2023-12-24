using Codebreaker.Data.Cosmos;
using Codebreaker.Data.SqlServer;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.GameAPIs.Data;

public static class StartupExtensions
{
    public static void AddDataStores(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.AddSqlServerDbContext<GamesSqlServerContext>("GamesSqlServerConnection", configureDbContextOptions: options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.Services.AddScoped<IGamesRepository, SqlServerProxy>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            string connectionString = builder.Configuration.GetConnectionString("GamesCosmosConnection") ?? throw new InvalidOperationException("Could not find GamesCosmosConnection");
            builder.AddCosmosDbContext<GamesCosmosContext>(connectionString, "datbasename", configureDbContextOptions: options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.Services.AddScoped<IGamesRepository, CosmosProxy>();
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {

            case "Cosmos":
                ConfigureCosmos(builder);
                break;
            case "SqlServer":
                ConfigureSqlServer(builder);
                break;
            default:
                ConfigureInMemory(builder );
                break;
        }
    }
}
