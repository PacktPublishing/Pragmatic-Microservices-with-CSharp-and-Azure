namespace Codebreaker.Live.Endpoints;

public class LiveHub(ILogger<LiveHub> logger) : Hub
{
    public async Task SubscribeToGameCompletions(string gameType)
    {
        logger.ClientSubscribed(Context.ConnectionId, gameType);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameType);
    }

    public async Task UnsubscribeFromGameCompletions(string gameType)
    {
        logger.ClientUnsubscribed(Context.ConnectionId, gameType);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameType);
    }
}
