using System.Text.Json;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Handles reporting of game end events asynchronously, sending game summary data to a specified endpoint.
/// </summary>
/// <param name="httpClient">Facilitates sending HTTP requests to the server for reporting game events.</param>
/// <param name="logger">Records errors encountered during the reporting process for later analysis.</param>
public class LiveReportClient(HttpClient httpClient, ILogger<LiveReportClient> logger) : IGameReport
{
    private readonly static JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Reports the end of a game by sending the game summary to a specified endpoint.
    /// </summary>
    /// <param name="gameSummary">Contains details about the completed game to be reported.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>This method does not return a value.</returns>
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
