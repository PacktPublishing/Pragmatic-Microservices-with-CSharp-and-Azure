using Azure.Messaging.EventHubs;

using Codebreaker.GameAPIs.Models;
using Codebreaker.Ranking.Data;

namespace Codebreaker.Ranking.Services;

// use this again after fixing Enrich
// public class GameSummaryEventProcessor(EventProcessorClient client, IDbContextFactory<RankingsContext> factory, ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
public class GameSummaryEventProcessor(
    EventProcessorClient client, 
    IServiceScopeFactory serviceScopeFactory, 
    ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
{
    /// <summary>
    /// Starts asynchronous processing of events, handling both event processing and errors.
    /// </summary>
    /// <param name="cancellationToken">Used to signal cancellation of the processing operation if needed.</param>
    /// <returns>Returns a task that represents the asynchronous operation.</returns>
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

        try
        {
            await client.StartProcessingAsync(cancellationToken);
        }
        catch (AggregateException ex)
        {
            logger.LogError(ex, "Error starting event processing using Azure Event Hub: {error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Stops the processing of tasks.
    /// </summary>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a task representing the asynchronous stop operation.</returns>
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
