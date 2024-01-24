namespace Codebreaker.GameAPIs.Client.Models;

/// <summary>
/// The result of a game move
/// </summary>
/// <param name="Id">The unique game identifier</param>
/// <param name="GameType">The game type with one of the <see cref="GameType"/>enum values</param>
/// <param name="MoveNumber">The move number for which the results are returned</param>
/// <param name="Ended">Did the game end?</param>
/// <param name="IsVictory">Did you win?</param>
/// <param name="Results">A string array containing the resulting key peg values</param>
internal record class UpdateGameResponse(
    Guid Id,
    GameType GameType,
    int MoveNumber,
    bool Ended,
    bool IsVictory,
    string[] Results);
