namespace Codebreaker.GameAPIs.Client.Models;

public class Move(Guid id, int moveNumber)
{
    public Guid Id { get; private set; } = id;

    /// <summary>
    /// The move number for this move within the associated game.
    /// </summary>
    public int MoveNumber { get; private set; } = moveNumber;

    /// <summary>
    /// The guess pegs from the user for this move.
    /// </summary>
    public required string[] GuessPegs { get; init; }
    /// <summary>
    /// The result from the analyer for this move based on the associated game that contains the move.
    /// </summary>
    public required string[] KeyPegs { get; init; }

    public override string ToString() => $"{MoveNumber}. " +
        $"{string.Join('#', GuessPegs)} : " +
        $"{string.Join('#', KeyPegs)}";
}
