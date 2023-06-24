using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Algorithms.Tests;

public class MockColorGame : IGame<ColorField, ColorResult>
{
    public Guid GameId { get; init; }
    public int NumberPositions { get; init; }
    public int MaxMoves { get; init; }
    public DateTime? EndTime { get; set; }
    public bool Won { get; set; }

    public required ILookup<string, string> FieldValues { get; init; }
    public required ColorField[] Codes { get; init; }

    public DateTime StartTime { get; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; }
    public required string GameType { get; set; }
}
