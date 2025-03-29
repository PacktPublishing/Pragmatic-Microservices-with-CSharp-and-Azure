using Azure.Messaging.EventHubs;

using Codebreaker.GameAPIs.Models;
using Codebreaker.Ranking.Data;

namespace Codebreaker.Ranking.Services;

// public class GameSummaryEventProcessor(EventProcessorClient client, IDbContextFactory<RankingsContext> factory, ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
public class GameSummaryEventProcessor(
    EventProcessorClient client, 
    IServiceScopeFactory serviceScopeFactory, 
    ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
{
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        client.ProcessEventAsync += async (args) =>
        {
            logger.LogInformation("Processing event");

            GameSummary? summary = args.Data.EventBody.ToObjectFromJson<GameSummary>();

            if (summary is null)
            {
                logger.LogWarning("Failed to deserialize game summary from event body");
                return;
            }
            logger.LogInformation("Received game completion event for game {gameId}", summary.Id);
            // using var context = await factory.CreateDbContextAsync(cancellationToken);
            using var scope = serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<RankingsContext>();

            await context.AddGameSummaryAsync(summary, cancellationToken);
            await args.UpdateCheckpointAsync(cancellationToken);
        };

        client.ProcessErrorAsync += (args) =>
        {
            logger.LogError(args.Exception, "Error processing event, {error}", args.Exception.Message);
            return Task.CompletedTask;
        };

        await client.StartProcessingAsync(cancellationToken);
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return client.StopProcessingAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Error}", ex.Message);
            throw;
        }
    }
}
