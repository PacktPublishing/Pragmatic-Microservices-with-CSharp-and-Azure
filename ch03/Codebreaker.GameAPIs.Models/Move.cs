namespace Codebreaker.GameAPIs.Models;

public abstract class Move(Guid gameId, Guid moveId, int moveNumber)
{
    public Guid GameId { get; private set; } = gameId;
    public Guid MoveId { get; private set; } = moveId;
    public int MoveNumber { get; private set; } = moveNumber;

    public override string ToString() => $"{GameId}: {MoveNumber}";
}

public class Move<TField, TResult>(Guid gameId, Guid moveId, int moveNumber)
    : Move(gameId, moveId, moveNumber)
    where TResult: struct
    where TField : IParsable<TField>
{
    public required ICollection<TField> GuessPegs { get; init; }
    public required TResult? KeyPegs { get; init; }

    public override string ToString() => $"{MoveNumber}, {string.Join(".", GuessPegs)}";
}
