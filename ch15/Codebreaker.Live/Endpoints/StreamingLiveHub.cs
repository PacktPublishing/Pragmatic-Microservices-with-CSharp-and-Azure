using Azure.Messaging.EventHubs.Consumer;

using Microsoft.Extensions.Azure;

using System.Runtime.CompilerServices;

namespace Codebreaker.Live.Endpoints;

/// <summary>
/// Streams game completion summaries based on a specified game type from event data.
/// </summary>
/// <param name="consumerClient">Used to read events asynchronously from a specified event hub.</param>
/// <param name="logger">Facilitates logging of processing events and errors during the subscription.</param>
public class StreamingLiveHub(EventHubConsumerClient consumerClient, ILogger<StreamingLiveHub> logger) : Hub
{
    /// <summary>
    /// Streams game completion summaries for a specified game type asynchronously.
    /// </summary>
    /// <param name="gameType">Specifies the type of game to filter the completion summaries.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Yields game summaries that match the specified game type.</returns>
    public async IAsyncEnumerable<GameSummary> SubscribeToGameCompletions(string gameType, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (PartitionEvent ev in consumerClient.ReadEventsAsync(cancellationToken))
        {
            GameSummary? gameSummary;
            try
            {
                logger.ProcessingGameCompletionEvent();
                gameSummary = ev.Data.EventBody.ToObjectFromJson<GameSummary>();
                if (gameSummary is null)
                {
                    logger.LogError("Failed to deserialize game summary from event body.");
                    continue;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorProcessingGameCompletionEvent(ex, ex.Message);
                continue;
            }

            if (gameSummary.GameType == gameType)
            {
                yield return gameSummary;
            }
            else
            {
                continue;
            }
        }
    }
}
