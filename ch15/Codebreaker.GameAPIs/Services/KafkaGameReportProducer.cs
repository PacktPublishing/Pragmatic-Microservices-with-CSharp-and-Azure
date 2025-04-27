using Confluent.Kafka;

using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Handles the production of game reports to Kafka and implements IDisposable for resource management.
/// </summary>
/// <param name="producerClient">Used to send messages to Kafka topics for reporting game events.</param>
/// <param name="logger">Facilitates logging of game completion events for tracking and debugging purposes.</param>
public sealed class KafkaGameReportProducer(
    IProducer<string, string> producerClient, 
    ILogger<KafkaGameReportProducer> logger) 
    : IGameReport, IDisposable
{
    public void Dispose()
    {
        producerClient.Flush(TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// Reports the end of a game by sending its summary to specified topics asynchronously.
    /// </summary>
    /// <param name="game">Contains details about the completed game, including its unique identifier and serialized data.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed, providing control over the asynchronous task.</param>
    /// <returns>Completes a task indicating that the reporting process has finished.</returns>
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
}
