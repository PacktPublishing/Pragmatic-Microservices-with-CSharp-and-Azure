using Codebreaker.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Codebreaker.Live.Endpoints;

public class GRPCLiveGameService(IHubContext<LiveHub> hubContext, ILogger<GRPCLiveGameService> logger) : ReportGame.ReportGameBase
{
    async public override Task<Empty> ReportGameCompleted(ReportGameCompletedRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received game ended {type} {gameid}", request.GameType, request.Id);
        await hubContext.Clients.Group(request.GameType).SendAsync("GameCompleted", request.ToGameSummary());
        return new Empty();
    }
}
