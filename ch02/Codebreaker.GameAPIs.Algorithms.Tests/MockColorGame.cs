using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Algorithms.Tests;

public class MockColorGame : IGame<ColorField, ColorResult>
{
    public Guid GameId { get; init; }
    public int Holes { get; init; }
    public int MaxMoves { get; init; }
    public DateTime? EndTime { get; set; }
    public bool Won { get; set; }

    public required IEnumerable<ColorField> Fields { get; init; }
    public required ICollection<ColorField> Codes { get; init; }

    public ICollection<IMove<ColorField, ColorResult>> Moves { get; } = new List<IMove<ColorField, ColorResult>>();
 
    public DateTime StartTime { get; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; }
    public required string GameType { get; set; }

    public IMove<ColorField, ColorResult> CreateMove(IEnumerable<ColorField> fields, ColorResult result, int moveNumber)
    {
        return new MockColorMove() { GuessPegs = fields.ToList(), KeyPegs = result, MoveNumber = moveNumber };
    }
}

public class MockColorMove : IMove<ColorField, ColorResult>
{
    public required int MoveNumber { get; init; }
    public required ICollection<ColorField> GuessPegs { get; init; }
    public required ColorResult? KeyPegs { get; init; }
}
