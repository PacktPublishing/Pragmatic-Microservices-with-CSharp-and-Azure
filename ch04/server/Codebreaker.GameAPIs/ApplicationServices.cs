// using Codebreaker.Data.PostgreSQL;

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

        //static void ConfigurePostgres(IHostApplicationBuilder builder)
        //{
        //    var connectionString = builder.Configuration.GetConnectionString("CodebreakerPostgres") ?? throw new InvalidOperationException("Could not read SQL Server connection string");

        //    builder.Services.AddNpgsqlDataSource(connectionString, dataSourceBuilder =>
        //    {
        //        dataSourceBuilder.EnableDynamicJson(); 
        //    });

        //    builder.Services.AddDbContextPool<IGamesRepository, GamesPostgreSQLContext>(options =>
        //    {
        //        options.UseNpgsql(connectionString);
        //        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        //    });
        //    builder.EnrichNpgsqlDbContext<GamesPostgreSQLContext>();
        //}

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

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {
            case "Cosmos":
                ConfigureCosmos(builder);
                break;
            case "SqlServer":
                ConfigureSqlServer(builder);
                break;
            //case "Postgres":
            //    ConfigurePostgres(builder);
            //    break;
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
        //else if (dataStore == "Postgres")
        //{
        //    try
        //    {
        //        using var scope = app.Services.CreateScope();
        //        var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
        //        if (repo is GamesPostgreSQLContext context)
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
        else if (dataStore == "Cosmos")
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