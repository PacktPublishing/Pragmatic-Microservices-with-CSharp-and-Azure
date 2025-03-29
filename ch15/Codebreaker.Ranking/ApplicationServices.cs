using Codebreaker.Ranking.Data;
using Codebreaker.Ranking.Endpoints;
using Codebreaker.Ranking.Services;

namespace Codebreaker.Ranking;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        string? mode = builder.Configuration["StartupMode"];
        if (mode == "OnPremises")
        {
            builder.AddKafkaConsumer<string, string>("kafkamessaging", settings =>
            {
               settings.Config.GroupId = "Ranking"; 
            });

            builder.Services.AddSingleton<IGameSummaryProcessor, GameSummaryKafkaConsumer>();
        }
        else
        {
            builder.AddKeyedAzureBlobClient("checkpoints");

            builder.AddAzureEventProcessorClient("games", settings =>
            {
                settings.EventHubName = "games";
                settings.BlobClientServiceKey = "checkpoints";
            });

            builder.Services.AddSingleton<IGameSummaryProcessor, GameSummaryEventProcessor>();
        }

        // removed the EnrichAPI because of simulator issues
        //builder.Services.AddDbContextFactory<RankingsContext>(options =>
        //{
        //    string connectionString = builder.Configuration.GetConnectionString("codebreakercosmos") ?? throw new InvalidOperationException("Could not read the Cosmos connection-string");
        //    options.UseCosmos(connectionString, "codebreaker");
        //});

        //builder.EnrichCosmosDbContext<RankingsContext>();

        builder.AddCosmosDbContext<RankingsContext>("RankingV3", "codebreaker");

        builder.Services.AddScoped<IRankingsRepository, DataContextProxy<RankingsContext>>();
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapRankingEndpoints();
        return app;
    }

    internal static bool IsDatabaseUpdateComplete { get; private set; } = false;

    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        // Creating the database and the container is now done with the AppHost
        //try
        //{
        //    using var scope = app.Services.CreateScope();
        //    // TODO: update with .NET Aspire Preview 4
        //    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RankingsContext>>();
        //    using var context = await factory.CreateDbContextAsync();

        //    bool created = await context.Database.EnsureCreatedAsync();
        //    app.Logger.LogInformation("Database created: {created}", created);
            
        //}
        //catch (Exception ex)
        //{
        //    app.Logger.LogError(ex, "Error updating database");
        //    throw;
        //}

        IsDatabaseUpdateComplete = true;
        await Task.CompletedTask;
    }
}
