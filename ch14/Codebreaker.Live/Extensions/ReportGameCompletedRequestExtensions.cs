using Codebreaker.Grpc;

namespace Codebreaker.Live.Extensions;

public static class ReportGameCompletedRequestExtensions
{
    public static GameSummary ToGameSummary(this ReportGameCompletedRequest request)
    {
        Guid id = Guid.Parse(request.Id);
        DateTime startTime = request.StartTime.ToDateTime();
        TimeSpan duration = request.Duration.ToTimeSpan();
        return new GameSummary(id, request.GameType, request.PlayerName, request.IsCompleted, request.IsVictory, request.NumberMoves, startTime, duration);
    }
}
