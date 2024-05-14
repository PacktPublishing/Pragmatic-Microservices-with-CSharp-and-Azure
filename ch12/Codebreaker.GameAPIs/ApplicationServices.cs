using Microsoft.Azure.Cosmos;
using System.Net;
using System.Security.Authentication;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextPool<IGamesRepository, GamesSqlServerContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("CodebreakerSql") ?? throw new InvalidOperationException("Could not read SQL Server connection string");
                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.EnrichSqlServerDbContext<GamesSqlServerContext>(static settings =>
            {
            });
        }

        // TODO: remove certificate workaround when the emulator is fixed
        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("cosmos") ?? throw new InvalidOperationException("Could not read Cosmos connection string");
                options.UseCosmos(connectionString, "codebreaker", cosmosOptions =>
                {
                    //cosmosOptions.HttpClientFactory(() =>
                    //    new HttpClient(new HttpClientHandler
                    //    {
                    //        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    //    }));
                    ////cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(10));
                    //cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.EnrichCosmosDbContext<GamesCosmosContext>(settings =>
            {

            });

            // TODO: cosmos workaround
            // ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        static void ConfigureDistributedMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddTransient<IGamesRepository, DistributedMemoryGamesRepository>();
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
            case "DistributedMemory":
                ConfigureDistributedMemory(builder);
                break;
            default:
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();

        builder.AddRedisDistributedCache("redis");

#if DEBUG
        // TODO: remove with preview 4, workaround for Cosmos emulator
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
#endif
    }

    private static bool s_IsDatabaseUpdateComplete = false;
    public static bool IsDatabaseUpdateComplete
    {
        get => s_IsDatabaseUpdateComplete;
    }

    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        var dataStore = app.Configuration["DataStore"] ?? "InMemory";

        if (dataStore == "SqlServer")
        {

            try
            {
                using var scope = app.Services.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesSqlServerContext context)
                {
                    await context.Database.MigrateAsync();
                    app.Logger.LogInformation("SQL Server database updated");
                    // add a delay to try out /health checks
                    // await Task.Delay(TimeSpan.FromSeconds(25));
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }

        // The database is created from the AppHost AddDatabase method. The Cosmos container is created here - if it doesn't exist yet.
        if (dataStore == "Cosmos")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesCosmosContext context)
                {
                    bool created = await context.Database.EnsureCreatedAsync();
                    app.Logger.LogInformation("Cosmos database created: {created}", created);
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }

        s_IsDatabaseUpdateComplete = true;
    }

    // TODO: temporary workaround to wait for Cosmos emulator to be available
    public static async Task WaitForEmulatorToBeRadyAsync(this WebApplication app)
    {
        if (app.Configuration["DataStore"] != "Cosmos")
        {
            return;
        }
        bool succeeded = false;
        int maxRetries = 30;
        int i = 0;
        HttpClient client = new();
        string cosmosConnection = app.Configuration.GetConnectionString("codebreakercosmos") ?? throw new InvalidOperationException();
        var ix1 = cosmosConnection.IndexOf("https");
        var ix2 = cosmosConnection.IndexOf(";DisableServer");
        string url = cosmosConnection[ix1..ix2];
        while (!succeeded && i++ < maxRetries)
        {
            try
            {
                await Task.Delay(5000);
                await client.GetAsync(url);
                succeeded = true;
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "{error}", ex.Message);
                if (ex.InnerException is not null)
                {
                    app.Logger.LogWarning(ex.InnerException, "{error}", ex.InnerException.Message);
                }
                if (ex.InnerException is AuthenticationException)
                {
                    succeeded = true;  // let's be ok with untrusted root with the emulator, we ignore this with the Cosmos config
                }
            }
        }
    }
}
