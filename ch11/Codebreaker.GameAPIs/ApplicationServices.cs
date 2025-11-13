using Codebreaker.GameAPIs.Infrastructure;
using Codebreaker.ServiceDefaults;

using System.Diagnostics;

using static Codebreaker.ServiceDefaults.ServiceNames;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMetrics();

        builder.Services.AddOpenTelemetry()
            .WithMetrics(m => m.AddMeter(GamesMetrics.MeterName));

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
                var connectionString = builder.Configuration.GetConnectionString(SqlDatabaseName) ?? throw new InvalidOperationException("Could not read SQL Server connection string");
                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.EnrichSqlServerDbContext<GamesSqlServerContext>();
        }

        //static void ConfigurePostgres(IHostApplicationBuilder builder)
        //{
        //    var connectionString = builder.Configuration.GetConnectionString(PostgresDatabaseName) ?? throw new InvalidOperationException("Could not read SQL Server connection string");

        //    builder.Services.AddNpgsqlDataSource(connectionString, dataSourceBuilder =>
        //    {
        //        dataSourceBuilder.EnableDynamicJson();
        //    });

        //    builder.Services.AddDbContextPool<IGamesRepository, GamesPostgresContext>(options =>
        //    {
        //        options.UseNpgsql(connectionString);
        //        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        //    });
        //    builder.EnrichNpgsqlDbContext<GamesPostgresContext>();
        //}

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.AddCosmosDbContext<GamesCosmosContext>(CosmosContainerName, CosmosDatabaseName);

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();

            // TODO: removed the Enrich API, and added back the DataContextProxy
            //builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            //{
            //    var connectionString = builder.Configuration.GetConnectionString("codebreakercosmos") ?? throw new InvalidOperationException("Could not read Cosmos connection string");
            //    options.UseCosmos(connectionString, "GamesV3", cosmosOptions =>
            //    {
            //    });
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //});

            // builder.EnrichCosmosDbContext<GamesCosmosContext>();
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        DataStoreType dataStore = builder.Configuration.GetDataStore();
        switch (dataStore)
        {
            case DataStoreType.Cosmos:
                ConfigureCosmos(builder);
                break;
            case DataStoreType.SqlServer:
                ConfigureSqlServer(builder);
                break;
            //case DataStoreType.Postgres:
            //    ConfigurePostgres(builder);
            //    break;
            default:
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();
        builder.AddApplicationTelemetry();
    }

    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        var dataStore = app.Configuration.GetDataStore();
        if (dataStore == DataStoreType.SqlServer)
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
        //else if (dataStore == DataStoreType.Postgres)
        //{
        //    try
        //    {
        //        using var scope = app.Services.CreateScope();
        //        var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
        //        if (repo is GamesPostgresContext context)
        //        {
        //            // TODO: migrations might be done in another sprint
        //            // for now, just ensure the database is created
        //            await context.Database.EnsureCreatedAsync();
        //            app.Logger.LogInformation("PostgreSQL database created");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        app.Logger.LogError(ex, "Error updating database");
        //        throw;
        //    }
        //}
        else if (dataStore == DataStoreType.Cosmos)
        {
            // with .NET Aspire 9.1 APIs, the container can be created with the app-model!
            // This code is only needed if the local installed Cosmos DB emulator is used
            // Instead of enabling this code, the database and container can be created using the Cosmos DB Emulator Data Explorer
            //try
            //{
            //    using var scope = app.Services.CreateScope();
            //    var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
            //    if (repo is GamesCosmosContext context)
            //    {
            //        bool created = await context.Database.EnsureCreatedAsync();
            //        app.Logger.LogInformation("Cosmos database created: {created}", created);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    app.Logger.LogError(ex, "Error updating database");
            //    throw;
            //}
        }
    }
}
