namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.AddSqlServerDbContext<GamesSqlServerContext>("CodebreakerSql",
                configureSettings: static settings =>
                {
                    settings.Metrics = true;
                    settings.Tracing = true;
                    settings.HealthChecks = true;
                },
                configureDbContextOptions: static options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });
            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesSqlServerContext>>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            // TODO: workaround for preview 3 to use the Cosmos emulator - remove with preview 4
            // azd up uses a debug build - just uncomment this workaround to run this on the local machine
//#if DEBUG
//            builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
//            {
//                options.UseCosmos("AccountEndpoint = http://localhost:8082/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "codebreaker",
//                    cosmosOptions =>
//                    {
//                        cosmosOptions.HttpClientFactory(() => new HttpClient(new HttpClientHandler()
//                        {
//                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//                        }));
//                        cosmosOptions.ConnectionMode(ConnectionMode.Gateway);
//                    });
//            });
//#else
            builder.AddCosmosDbContext<GamesCosmosContext>("codebreaker", "codebreaker",
                configureSettings: settings =>
                {
                    settings.IgnoreEmulatorCertificate = true;
                    settings.Metrics = true;
                    settings.Tracing = true;
                },
                configureDbContextOptions: options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();
// #endif
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
        builder.Services.AddHttpClient<ILiveClient, APILiveClient>(client =>
        {
            client.BaseAddress = new Uri("http://live");
        });

        builder.AddRedisDistributedCache("redis");
    }

    private static bool s_IsDatabaseUpdateComplete = false;
    public static bool IsDatabaseUpdateComplete
    {
        get => s_IsDatabaseUpdateComplete;
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

                // add a delay to try out /health checks
                await Task.Delay(TimeSpan.FromSeconds(25));
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
                throw;
            }
        }

        s_IsDatabaseUpdateComplete = true;
    }
}
