namespace Codebreaker.Live.Endpoints;

public class LiveHub : Hub
{
    public async Task RegisterGameCompletions(string gameType, CancellationToken cancellationToken)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameType, cancellationToken);
    }

    //public async Task SendGameCompletion(GameSummary gameSummary)
    //{
    //    await Clients.Group(gameSummary.GameType).SendGameSummaryAsync("GameCompletion", gameSummary);
    //}
}
