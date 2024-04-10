using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

using System.Text;
using System.Text.Json;

namespace Codebreaker.BotQ.Endpoints;

public class BotQueueClient(QueueServiceClient client, CodeBreakerTimer timer, ILogger<BotQueueClient> logger)
{
    public async Task RunAsync()
    {   
        var queueClient = client.GetQueueClient("botqueue");
        await queueClient.CreateIfNotExistsAsync();


        QueueProperties properties = await queueClient.GetPropertiesAsync();
        if (properties.ApproximateMessagesCount > 0)
        {
            logger.LogInformation("Queue has {count} messages", properties.ApproximateMessagesCount);
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync();
            foreach (var encMessage in messages)
            {
                byte[] bytes = Convert.FromBase64String(encMessage.MessageText);
                string message = Encoding.UTF8.GetString(bytes);
                logger.LogInformation("Received queue message: {message}", message);

                BotMessage? bm = JsonSerializer.Deserialize<BotMessage>(message);
                if (bm is null)
                {
                    logger.LogWarning("Failed to deserialize message: {message}", message);
                    continue;
                }
                Guid id = timer.Start(bm.Delay, bm.Count, bm.ThinkTime);
                logger.LogInformation("Started playing games with game sequence {id}", id);

                await queueClient.DeleteMessageAsync(encMessage.MessageId, encMessage.PopReceipt);
            }
        }
    }
}

public record class BotMessage(int Count, int Delay, int ThinkTime);
