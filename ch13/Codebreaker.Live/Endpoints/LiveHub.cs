namespace Codebreaker.Live.Endpoints;

public class LiveHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task RegisterGameCompletions(string gameType)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameType);
    }
}
