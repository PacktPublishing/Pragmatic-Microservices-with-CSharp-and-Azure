using Google.Protobuf.WellKnownTypes;

namespace Codebreaker.Ranking.Data;

public class GameSummary1(
    Guid id,
    string gameType,
    string playerName,
    bool isCompleted,
    bool isVictory,
    int numberMoves,
    DateTime startTime,
    TimeSpan duration)
{
    public Guid Id { get; private set; } = id;
    public string GameType { get; private set; } = gameType;
    public string PlayerName { get; private set; } = playerName;
    public bool IsCompleted { get; private set; } = isCompleted;
    public bool IsVictory { get; private set; } = isVictory;
    public int NumberMoves { get; private set; } = numberMoves;
    public DateTime StartTime { get; private set; } = startTime;
    public TimeSpan Duration { get; private set; } = duration;
    public override string ToString() => $"{Id}:{GameType}, victory: {IsVictory}, duration: {Duration}";
}
