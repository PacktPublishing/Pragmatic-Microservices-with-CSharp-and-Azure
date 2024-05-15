using Azure.Messaging.EventHubs;

using Codebreaker.GameAPIs.Models;
using Codebreaker.Ranking.Data;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Ranking.Services;

public class GameSummaryEventProcessor(EventProcessorClient client, IDbContextFactory<RankingsContext> factory, ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
{
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        client.ProcessEventAsync += async (args) =>
        {
            logger.LogInformation("Processing event");

            GameSummary summary = args.Data.EventBody.ToObjectFromJson<GameSummary>();

            logger.LogInformation("Received game completion event for game {gameId}", summary.Id);
            using var context = await factory.CreateDbContextAsync(cancellationToken);

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
