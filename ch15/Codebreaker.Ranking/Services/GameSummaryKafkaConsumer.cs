using Codebreaker.GameAPIs.Models;
using Codebreaker.Ranking.Data;

using Confluent.Kafka;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace Codebreaker.Ranking.Services;

// use this again after fixing Enrich
// public class GameSummaryKafkaConsumer(IConsumer<string, string> kafkaClient, IDbContextFactory<RankingsContext> factory, ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
public class GameSummaryKafkaConsumer(IConsumer<string, string> kafkaClient, IServiceScopeFactory serviceScopeFactory, ILogger<GameSummaryEventProcessor> logger) : IGameSummaryProcessor
{
    /// <summary>
    /// Starts processing messages from a Kafka topic and saves game summaries to a database.
    /// </summary>
    /// <param name="cancellationToken">Allows the operation to be cancelled, providing a way to stop processing if needed.</param>
    /// <returns>Completes when the processing is finished or cancelled.</returns>
    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {

        kafkaClient.Subscribe("ranking");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = kafkaClient.Consume(cancellationToken);
                    var value = result.Message.Value;
                    var summary = JsonSerializer.Deserialize<GameSummary>(value);

                    if (summary is null)
                    {
                        logger.LogError("Deserialized null GameSummary");
                        continue;
                    }

                    // using var context = await factory.CreateDbContextAsync(cancellationToken);
                    using var scope = serviceScopeFactory.CreateScope();
                    using var context = scope.ServiceProvider.GetRequiredService<RankingsContext>();
                    await context.AddGameSummaryAsync(summary, cancellationToken);
                }
                catch (ConsumeException ex) when (ex.HResult == -2146233088)
                {
                    logger.LogWarning("Consume exception {Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Processing was cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Stops the processing of messages.
    /// </summary>
    /// <param name="cancellationToken">Used to signal cancellation of the operation if needed.</param>
    /// <returns>Returns a completed task indicating the operation has finished.</returns>
    public Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        kafkaClient.Unsubscribe();
        return Task.CompletedTask;
    }
}
