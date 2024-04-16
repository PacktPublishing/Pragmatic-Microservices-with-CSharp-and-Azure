using Confluent.Kafka;

using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

public class KafkaLiveReportProducer(IProducer<string, string> producerClient, ILogger<KafkaLiveReportProducer> logger) 
    : ILiveReportClient
{
    public async Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default)
    {
        Message<string, string> message = new()
        {
            Key = game.Id.ToString(),
            Value = JsonSerializer.Serialize(game)
        };

        await producerClient.ProduceAsync("gamesummary", message, cancellationToken);

        logger.GameCompletionSent(game.Id, "Kafka");
    }
}
