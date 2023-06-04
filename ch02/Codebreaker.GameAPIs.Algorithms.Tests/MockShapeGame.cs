using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Algorithms.Tests;

public class MockShapeGame : IGame<ShapeAndColorField, ShapeAndColorResult>
{
    public int Holes { get; init; }
    public int MaxMoves { get; init; }
    public DateTime? EndTime { get; set; }
    public bool Won { get; set; }

    public required IEnumerable<ShapeAndColorField> Fields { get; init; }
    public required ICollection<ShapeAndColorField> Codes { get; init; }

    private readonly List<IMove<ShapeAndColorField, ShapeAndColorResult>> _moves = new();
    public ICollection<IMove<ShapeAndColorField, ShapeAndColorResult>> Moves => _moves;

    public DateTime StartTime { get; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; }
    public required string GameType { get; init; }

    public IMove<ShapeAndColorField, ShapeAndColorResult> CreateMove(IEnumerable<ShapeAndColorField> fields, ShapeAndColorResult result, int moveNumber)
    {
        return new MockShapeMove() { GuessPegs = fields.ToList(), KeyPegs = result, MoveNumber = moveNumber };
    }
}

public class MockShapeMove : IMove<ShapeAndColorField, ShapeAndColorResult>
{
    public required int MoveNumber { get; init; }
    public required ICollection<ShapeAndColorField> GuessPegs { get; init; }
    public ShapeAndColorResult? KeyPegs { get; init; }
}
