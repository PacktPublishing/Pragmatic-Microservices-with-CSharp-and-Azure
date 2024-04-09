using Codebreaker.Grpc;

using Grpc.Core;

using System.Net.Sockets;

namespace Codebreaker.GameAPIs.Services;

public class GrpcLiveReportClient(ReportGame.ReportGameClient client, ILogger<LiveReportClient> logger) : ILiveReportClient
{
    public async Task ReportGameEndedAsync(GameSummary gameSummary, CancellationToken cancellationToken = default)
    {
        try
        {
            ReportGameCompletedRequest request = gameSummary.ToReportGameCompletedRequest();
            await client.ReportGameCompletedAsync(request);
        }
        catch (Exception ex) when (ex is RpcException or SocketException)
        {
            logger.ErrorWritingGameCompletedEvent(gameSummary.Id, ex);
            // we don't want to rethrow the exception here, we just want to log it and move on
            // reporting game ended is not essential to the game play
        }
    }
}
