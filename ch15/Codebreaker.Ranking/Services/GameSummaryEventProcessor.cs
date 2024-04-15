using Azure.Messaging.EventHubs;

using Codebreaker.GameAPIs.Models;
using Codebreaker.Ranking.Data;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Ranking.Services;

public class GameSummaryEventProcessor(EventProcessorClient client, IDbContextFactory<RankingsContext> factory, ILogger<GameSummaryEventProcessor> logger)
{
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        client.ProcessEventAsync += async (args) =>
        {
            logger.LogInformation("Processing event");

            GameSummary1 summary = args.Data.EventBody.ToObjectFromJson<GameSummary1>();

            logger.LogInformation("Received game completion event for game {gameId}", summary.Id);
            var context = await factory.CreateDbContextAsync(cancellationToken);

            await Task.WhenAll(
                context.AddGameSummaryAsync(summary, cancellationToken),
                args.UpdateCheckpointAsync(cancellationToken));
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
            logger.LogError(ex, message: ex.Message);
            throw;
        }
    }
}
