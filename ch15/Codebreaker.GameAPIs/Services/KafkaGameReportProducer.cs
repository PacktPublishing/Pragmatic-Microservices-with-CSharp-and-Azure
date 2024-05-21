using Confluent.Kafka;

using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

public sealed class KafkaGameReportProducer(IProducer<string, string> producerClient, ILogger<KafkaGameReportProducer> logger) 
    : IGameReport, IDisposable
{
    public Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default)
    {
        Message<string, string> message = new()
        {
            Key = game.Id.ToString(),
            Value = JsonSerializer.Serialize(game)
        };

        string[] topics = ["ranking", "live"];
        foreach (var topic in topics)
        {
            _ = producerClient.ProduceAsync(topic, message, cancellationToken);
        }

        logger.GameCompletionSent(game.Id, "Kafka");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        producerClient.Flush(TimeSpan.FromSeconds(5));
    }
}
