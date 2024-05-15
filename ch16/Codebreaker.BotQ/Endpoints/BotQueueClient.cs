using static Codebreaker.BotQ.Endpoints.BotQueueClient;

namespace Codebreaker.BotQ.Endpoints;

public class BotQueueClient(QueueServiceClient client, CodebreakerTimer timer, ILogger<BotQueueClient> logger, IOptions<BotQueueClientOptions> options)
{
    public async Task RunAsync()
    {        
        var queueClient = client.GetQueueClient("botqueue");
        await queueClient.CreateIfNotExistsAsync();

        var deadLetterClient = client.GetQueueClient("dead-letter");
        await deadLetterClient.CreateIfNotExistsAsync();

        bool repeat = options.Value.Loop;
        do
        {
            await ProcessMessagesAsync(queueClient, deadLetterClient);
            await Task.Delay(options.Value.Delay);
        } while (repeat);
    }

    private async Task ProcessMessagesAsync(QueueClient queueClient, QueueClient deadLetterClient)
    {
        QueueProperties properties = await queueClient.GetPropertiesAsync();
        if (properties.ApproximateMessagesCount > 0)
        {
            logger.LogInformation("Queue has {count} messages", properties.ApproximateMessagesCount);
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync();
            foreach (var encodedMessage in messages)
            {
                try
                {
                    if (encodedMessage.DequeueCount > 3)
                    {
                        logger.MessageDequeueCountExceeded(encodedMessage.MessageId, encodedMessage.DequeueCount);
                        await deadLetterClient.SendMessageAsync(encodedMessage.MessageText);
                        await queueClient.DeleteMessageAsync(encodedMessage.MessageId, encodedMessage.PopReceipt);
                        continue;
                    }

                    byte[] bytes = Convert.FromBase64String(encodedMessage.MessageText);
                    string message = Encoding.UTF8.GetString(bytes);
                    logger.LogInformation("Received queue message: {message}", message);

                    var botMessage = JsonSerializer.Deserialize<BotMessage>(message);
                    if (botMessage is null)
                    {
                        logger.DeserializedNullMessage(encodedMessage.MessageText);
                        continue;
                    }
                    Guid id = timer.Start(botMessage.Delay, botMessage.Count, botMessage.ThinkTime);
                    logger.StartedPlayingGames(id);

                    await queueClient.DeleteMessageAsync(encodedMessage.MessageId, encodedMessage.PopReceipt);
                }
                catch (FormatException ex)
                {
                    logger.FailedToDecodeMessage(ex, encodedMessage.MessageText);
                }
                catch (JsonException ex)
                {
                    logger.FailedToDeserializeMessage(ex, encodedMessage.MessageText);
                    // but still continue with the other messages
                }
            }
        }
    }
}

public class BotQueueClientOptions
{
    public bool Loop { get; set; } = false;
    
    public int Delay { get; set; } = 1000;
}

public record class BotMessage(int Count, int Delay, int ThinkTime);
