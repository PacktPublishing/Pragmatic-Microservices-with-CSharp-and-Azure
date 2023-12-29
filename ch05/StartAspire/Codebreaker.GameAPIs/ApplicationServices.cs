using Codebreaker.Data.Cosmos;
using Codebreaker.Data.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            // TODO 4: configure to retrieve the SQL Server connection string via service discovery with .NET Aspire

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesSqlServerContext>>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.AddCosmosDbContext<GamesCosmosContext>("GamesCosmosConnection", "codebreaker",
                configureDbContextOptions: options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();
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
        builder.AddDatabaseMigrationHealthChecks();
    }

    public static void AddDatabaseMigrationHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddCheck("databasecreated", () =>
        {
            Console.WriteLine("My health check...");
            return s_databaseCreated ? HealthCheckResult.Healthy("database created") : HealthCheckResult.Degraded("database not created");
        }, ["ready"]);
    }

    private static bool s_databaseCreated = false;

    public static IApplicationBuilder CreateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();

        if (repo is GamesSqlServerContext sqlServerContext)
        {
            sqlServerContext.Database.Migrate();
        }
        s_databaseCreated = true;

        return app;
    }
}
