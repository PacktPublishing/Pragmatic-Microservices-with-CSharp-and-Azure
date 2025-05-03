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
            builder.AddCosmosDbContext<GamesCosmosContext>("GamesV3", "codebreaker");

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();

            // TODO: removed the Enrich API, and added back the DataContextProxy, check back with a later version of the Cosmos emulator
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
        else if (dataStore == "Postgres")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesPostgresContext context)
                {
                    // TODO: migrations might be done in another sprint
                    // for now, just ensure the database is created
                    await context.Database.EnsureCreatedAsync();
                    app.Logger.LogInformation("PostgreSQL database created");
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }

        // The database is created from the AppHost AddDatabase method. The Cosmos container is created here - if it doesn't exist yet.
        // The Cosmos database and container are now created using the app-model!
        else if (dataStore == "Cosmos")
        {
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
