using Codebreaker.Grpc;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Diagnostics;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMetrics();

        builder.Services.AddOpenTelemetry().WithMetrics(m => m.AddMeter(GamesMetrics.MeterName));

        builder.Services.AddSingleton<GamesMetrics>();

        builder.Services.AddKeyedSingleton("Codebreaker.GameAPIs", (services, _) => new ActivitySource("Codebreaker.GameAPIs", "1.0.0"));

        builder.Services.AddHealthChecks().AddCheck("databaseupdate", () =>
        {
            return IsDatabaseUpdateComplete ?
                HealthCheckResult.Healthy("DB update done") :
                HealthCheckResult.Degraded("DB update not ready");
        }, ["ready"]);
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
            builder.AddCosmosDbContext<GamesCosmosContext>("GamesV3", "codebreaker");

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();

            // removed the Enrich API, and added back the DataContextProxy
            // because of an issue with the Cosmos DB preview emulator: 
            // https://github.com/dotnet/aspire/issues/8177
            //builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            //{
            //    var connectionString = builder.Configuration.GetConnectionString("GamesV3") ?? throw new InvalidOperationException("Could not read Cosmos connection string");
            //    options.UseCosmos(connectionString, "gamesv3", cosmosOptions =>
            //    {
            //    });
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //});

            // builder.EnrichCosmosDbContext<GamesCosmosContext>(settings =>
            //{
            //});
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

        string? mode = builder.Configuration["StartupMode"];
        if (mode == "OnPremises")
        {
            builder.AddKafkaProducer<string, string>("kafkamessaging", settings =>
            {
                settings.Config.AllowAutoCreateTopics = true;
            });

            builder.Services.AddSingleton<IGameReport, KafkaGameReportProducer>();
        }
        else
        {
            builder.Services.AddScoped<IGameReport, EventHubReportProducer>();

            builder.AddAzureEventHubProducerClient("games", settings =>
            {
                settings.EventHubName = "games";
            });
        }

        builder.Services.AddGrpc();

        builder.Services.AddSingleton<IGameReport, GrpcLiveReportClient>()
            .AddGrpcClient<ReportGame.ReportGameClient>(client =>
            {
                client.Address = new Uri("https://live");
            });
        builder.AddRedisDistributedCache("redis");
    }

    internal static bool IsDatabaseUpdateComplete { get; private set; } = false;

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

                // add a delay to show /health checks
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
            // Creating the database and the container is now done with the AppHost
        }

        IsDatabaseUpdateComplete = true;
    }
}
