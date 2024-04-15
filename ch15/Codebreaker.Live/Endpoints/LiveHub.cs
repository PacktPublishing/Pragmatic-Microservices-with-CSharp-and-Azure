
namespace Codebreaker.Live.Endpoints;

// public class LiveHub(LiveGamesEventProcessor eventProcessor, LiveHubClientsCount clientsCounter, ILogger<LiveHub> logger) : Hub
public class LiveHub(LiveHubClientsCount clientsCounter, ILogger<LiveHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task SubscribeToGameCompletions(string gameType, CancellationToken cancellationToken = default)
    {
        logger.ClientSubscribed(Context.ConnectionId, gameType);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameType, cancellationToken);
        int numberClients = clientsCounter.AddClient(Context.ConnectionId);
        if (numberClients == 1)
        {
//            await eventProcessor.StartProcessingAsync();
        }
    }

    public async Task UnsubscribeFromGameCompletions(string gameType, CancellationToken cancellationToken = default)
    {
        logger.ClientUnsubscribed(Context.ConnectionId, gameType);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameType, cancellationToken);
        int numberClients = clientsCounter.RemoveClient(Context.ConnectionId);
        if (numberClients == 0)
        {
  //          await eventProcessor.StopProcessingAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.ClientDisconnected(Context.ConnectionId);
        int numberClients = clientsCounter.RemoveClient(Context.ConnectionId);
        if (numberClients == 0)
        {
    //        await eventProcessor.StopProcessingAsync();
        }
        await base.OnDisconnectedAsync(exception);
    }
}

public class LiveHubClientsCount
{
    private readonly ConcurrentDictionary<string, DateTime> _clients = new();

    public int AddClient(string connectionId)
    {
        _clients.AddOrUpdate(connectionId, DateTime.UtcNow, (_, _) => DateTime.UtcNow);
        return _clients.Count;
    }

    public int RemoveClient(string connectionId)
    {
       _clients.TryRemove(connectionId, out _);
       return _clients.Count;
    }
}