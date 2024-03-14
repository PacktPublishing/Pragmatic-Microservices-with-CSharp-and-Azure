namespace Codebreaker.GameAPIs.Models;

public record class GameSummary1
{
    public Guid Id { get; init; }
    public required string GameType { get; init; }
    public required string PlayerName { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsVictory { get; init; }
    public DateTime StartTime { get; init; }
    public TimeSpan Duration { get; init; }

    public override string ToString()
    {
        return $"{Id}:{GameType}, victory: {IsVictory}"; //, duration: {Duration}";
    }
}
