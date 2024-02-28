using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

public class APILiveClient(HttpClient httpClient, ILogger<APILiveClient> logger) : ILiveClient
{
    public async Task ReportGameEndedAsync(Game game, CancellationToken cancellationToken = default)
    {
        try
        {
            await httpClient.PostAsJsonAsync("/live/game", game, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            logger.ErrorWritingGameCompletedEvent(game.Id, ex);
            // we don't want to rethrow the exception here, we just want to log it and move on
            // reporting game ended is not essential to the game play
        }
    }
}
