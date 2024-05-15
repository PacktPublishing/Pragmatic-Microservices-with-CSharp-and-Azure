using Codebreaker.Data.Cosmos;
using Codebreaker.Data.SqlServer;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextPool<IGamesRepository, GamesSqlServerContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("codebreaker") ??
                    throw new InvalidOperationException("SQL Server connection string not configured");
                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.EnrichSqlServerDbContext<GamesSqlServerContext>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextPool<IGamesRepository, GamesCosmosContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("codebreakercosmos") ??
                    throw new InvalidOperationException("Cosmos connection string not configured");
                options.UseCosmos(connectionString, "codebreaker");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.EnrichCosmosDbContext<GamesCosmosContext>();
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
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();
    }

}
