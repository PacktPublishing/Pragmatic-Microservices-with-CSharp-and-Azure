using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

public class EventHubLiveReportClient(EventHubProducerClient producerClient, ILogger<EventHubLiveReportClient> logger) : ILiveReportClient
{
    public async Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default)
    {
        var data = BinaryData.FromString(JsonSerializer.Serialize(game));

        await producerClient.SendAsync([ new EventData(data) ], cancellationToken);

        logger.GameCompletionSent(game.Id, "Event Hub");
    }
}
