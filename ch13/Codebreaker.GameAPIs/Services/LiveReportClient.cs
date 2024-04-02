using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

public class LiveReportClient(HttpClient httpClient, ILogger<LiveReportClient> logger) : ILiveReportClient
{
    private readonly static JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task ReportGameEndedAsync(GameSummary gameSummary, CancellationToken cancellationToken = default)
    {
        try
        {
            await httpClient.PostAsJsonAsync("/live/game", gameSummary, options: s_jsonOptions, cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            logger.ErrorWritingGameCompletedEvent(gameSummary.Id, ex);
            // we don't want to rethrow the exception here, we just want to log it and move on
            // reporting game ended is not essential to the game play
        }
    }
}
