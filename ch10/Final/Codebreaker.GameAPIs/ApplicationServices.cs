using Codebreaker.Data.Cosmos;
using Codebreaker.Data.SqlServer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.AddSqlServerDbContext<GamesSqlServerContext>("CodebreakerSql",
                configureDbContextOptions: static options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });
            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesSqlServerContext>>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            // TODO: workaround for preview 3 to use the Cosmos emulator - remove with preview 4
#if DEBUG
            builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            {
                options.UseCosmos("AccountEndpoint = https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "codebreaker",
                 cosmosOptions =>
                 {
                     cosmosOptions.HttpClientFactory(() => new HttpClient(new HttpClientHandler()
                     {
                         ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                     }));
                     cosmosOptions.ConnectionMode(ConnectionMode.Gateway);
                 });
            });
#else
            builder.AddCosmosDbContext<GamesCosmosContext>("codebreaker", "codebreaker",
                configureSettings: static options =>
                {
                    options.IgnoreEmulatorCertificate = true;
                },
                configureDbContextOptions: static options =>
                {
                  
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();
#endif
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

#if DEBUG
        // TODO: remove with preview 4, workaround for Cosmos emulator
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
#endif
    }

    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        if (app.Configuration["DataStore"] == "SqlServer")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<GamesSqlServerContext>();

                // TODO: update with .NET Aspire Preview 4
                // var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
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
                // TODO: update with .NET Aspire Preview 4
                var repo = scope.ServiceProvider.GetRequiredService<GamesCosmosContext>();
                //                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
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
