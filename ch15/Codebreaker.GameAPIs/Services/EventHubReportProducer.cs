using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Codebreaker.GameAPIs.Services;

public class EventHubReportProducer(EventHubProducerClient producerClient, ILogger<EventHubReportProducer> logger) : IGameReport
{
    public async Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default)
    {
        var data = BinaryData.FromObjectAsJson(game);
        await producerClient.SendAsync([new EventData(data)], cancellationToken);

        logger.GameCompletionSent(game.Id, "Event Hub");
    }
}