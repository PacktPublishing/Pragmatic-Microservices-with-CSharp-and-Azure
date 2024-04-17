using Codebreaker.Grpc;

using Google.Protobuf.WellKnownTypes;

namespace Codebreaker.GameAPIs.Extensions;

public static class GameSummaryExtensions
{
    public static ReportGameCompletedRequest ToReportGameCompletedRequest(this GameSummary gameSummary)
    {
        return new ReportGameCompletedRequest()
        {
            Id = gameSummary.Id.ToString(),
            GameType = gameSummary.GameType,
            PlayerName = gameSummary.PlayerName,
            IsCompleted = gameSummary.IsCompleted,
            IsVictory = gameSummary.IsVictory,
            NumberMoves = gameSummary.NumberMoves,
            StartTime = Timestamp.FromDateTime(gameSummary.StartTime),
            Duration = Duration.FromTimeSpan(gameSummary.Duration)
        };
    }
}
