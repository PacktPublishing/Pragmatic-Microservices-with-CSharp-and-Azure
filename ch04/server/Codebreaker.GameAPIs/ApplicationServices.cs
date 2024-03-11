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
            builder.EnrichSqlServerDbContext<GamesSqlServerContext>();
        }

        // TODO: remove certificate workaround when the emulator is fixed
        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("codebreakercosmos") ?? throw new InvalidOperationException("Could not read Cosmos connection string");
                options.UseCosmos(connectionString, "codebreaker", cosmosOptions =>
                {
                    cosmosOptions.HttpClientFactory(() =>
                        new HttpClient(new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }));
                    //cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(10));
                    cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.EnrichCosmosDbContext<GamesCosmosContext>(settings =>
            {
               
            });

            // TODO: cosmos workaround
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
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
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }
        else if (dataStore == "Cosmos")
        {
            bool succeeded = false;
            int maxRetries = 30;
            int i = 0;
            while (!succeeded)
            {
                CancellationTokenSource cts = new(TimeSpan.FromSeconds(180));
                try
                {
                    using var scope = app.Services.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                    if (repo is GamesCosmosContext context)
                    {
                        bool created = await context.Database.EnsureCreatedAsync(cts.Token);
                        succeeded = true;
                        app.Logger.LogInformation("Cosmos database created: {created}", created);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is IOException ioEx)
                    {
                        app.Logger.LogWarning(ioEx, "IOException with emulator, {error}", ioEx.Message);
                        i++;
                        if (i >= maxRetries)
                        {
                            app.Logger.LogError(ioEx, "Error updating database");
                            throw;
                        }
                        await Task.Delay(5000); // it takes a minute!
                    }
                    else
                    {
                        app.Logger.LogError(ex, "Error updating database");
                        throw;
                    }
                }
            }
        }
    }

    // TODO: temporary workaround to wait for Cosmos emulator to be available
    public static async Task WaitForEmulatorToBeRadyAsync(this WebApplication app)
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

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
                app.Logger.LogWarning(ex, ex.Message);
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