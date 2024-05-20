using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMetrics();

        builder.Services.AddOpenTelemetry().WithMetrics(m => m.AddMeter(GamesMetrics.MeterName));

        builder.Services.AddSingleton<GamesMetrics>();

        const string ActivitySourceName = "Codebreaker.GameAPIs";
        const string ActivitySourceVersion = "1.0.0";

        builder.Services.AddKeyedSingleton(ActivitySourceName, (services, _) => 
            new ActivitySource(ActivitySourceName, ActivitySourceVersion));
    }


    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextPool<IGamesRepository, GamesSqlServerContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("CodebreakerSql") ??
                    throw new InvalidOperationException("Connection string CodebreakerSql not configured");
                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.EnrichSqlServerDbContext<GamesSqlServerContext>(settings =>
            {
            });
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("codebreakercosmos") ?? throw new InvalidOperationException("Could not read the Cosmos connection-string");
                options.UseCosmos(connectionString, "codebreaker");

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.EnrichCosmosDbContext<GamesCosmosContext>(settings =>
            {
            });
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {
            case "SqlServer":
                ConfigureSqlServer(builder);
                break;
            case "Cosmos":
                ConfigureCosmos(builder);
                break;
            default:
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();

        builder.AddApplicationTelemetry();
    }


    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        if (app.Configuration["DataStore"] == "SqlServer")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesSqlServerContext context)
                {
                    await context.Database.MigrateAsync();
                    app.Logger.LogInformation("Database updated");
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
            }
        }

        // The database is created from the AppHost AddDatabase method. The Cosmos container is created here - if it doesn't exist yet.
        if (app.Configuration["DataStore"] == "Cosmos")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesCosmosContext context)
                {
                    bool created = await context.Database.EnsureCreatedAsync();
                    app.Logger.LogInformation("Database created: {created}", created);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
            }
        }
    }
}
