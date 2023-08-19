namespace Codebreaker.GameAPIs.Algorithms.Tests;

public class MockShapeGame : IGame
{
    public Guid GameId { get; init; }
    public int NumberCodes { get; init; }
    public int MaxMoves { get; init; }
    public DateTime? EndTime { get; set; }
    public bool IsVictory { get; set; }

    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }
    public required string[] Codes { get; init; }

    public DateTime StartTime { get; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; }
    public required string GameType { get; init; }
}
