namespace Codebreaker.GameAPIs.Client.Models;

/// <summary>
/// Represents the move within a game <see cref="GameInfo"/ >with the guess pegs and key pegs.
/// </summary>
/// <param name="moveId"/>The unique identifier of the move. This is needed to reference the move.
/// <param name="moveNumber"/>The move number for this move within the associated game.
public class MoveInfo(Guid moveId, int moveNumber)
{
    public Guid MoveId { get; private set; } = moveId;

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
