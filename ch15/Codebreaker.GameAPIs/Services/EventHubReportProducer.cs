using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Handles reporting of game completion events to an Event Hub.
/// </summary>
/// <param name="producerClient">Facilitates sending event data to the Event Hub.</param>
/// <param name="logger">Records the completion of game reporting activities.</param>
public class EventHubReportProducer(EventHubProducerClient producerClient, ILogger<EventHubReportProducer> logger) : IGameReport
{
    /// <summary>
    /// Reports the end of a game by sending its summary to an event hub.
    /// </summary>
    /// <param name="game">Contains the details of the game that has ended.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Completes asynchronously without returning a value.</returns>
    public async Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default)
    {
        var data = BinaryData.FromObjectAsJson(game);
        await producerClient.SendAsync([new EventData(data)], cancellationToken);

        logger.GameCompletionSent(game.Id, "Event Hub");
    }
}