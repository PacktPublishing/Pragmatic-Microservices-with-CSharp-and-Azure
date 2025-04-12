using Codebreaker.Grpc;

using Grpc.Core;

using System.Net.Sockets;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Handles reporting the end of a game asynchronously while managing exceptions.
/// </summary>
/// <param name="client">Used to send the completed game report to the server.</param>
/// <param name="logger">Facilitates logging errors that occur during the reporting process.</param>
public class GrpcLiveReportClient(ReportGame.ReportGameClient client, ILogger<LiveReportClient> logger) : IGameReport
{
    public async Task ReportGameEndedAsync(GameSummary gameSummary, CancellationToken cancellationToken = default)
    {
        try
        {
            ReportGameCompletedRequest request = gameSummary.ToReportGameCompletedRequest();
            await client.ReportGameCompletedAsync(request, cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is RpcException or SocketException)
        {
            logger.ErrorWritingGameCompletedEvent(gameSummary.Id, ex);
            // we don't want to rethrow the exception here, we just want to log it and move on
            // reporting game ended is not essential to the game play
        }
    }
}
