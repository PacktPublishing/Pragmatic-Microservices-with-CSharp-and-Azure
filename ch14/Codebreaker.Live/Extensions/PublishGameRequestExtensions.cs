using Codebreaker.Grpc;

namespace Codebreaker.Live.Extensions;

public static class PublishGameRequestExtensions
{
    public static GameSummary ToGameSummary(this PublishGameRequest request)
    {
        Guid id = Guid.Parse(request.Id);
        DateTime startTime = new DateTime(request.StartTime);
        TimeSpan duration = new TimeSpan(request.Duration);
        return new GameSummary(id, request.GameType, request.PlayerName, request.IsComleted, request.IsVictory, request.NumberMoves, startTime, duration);
    }
}
