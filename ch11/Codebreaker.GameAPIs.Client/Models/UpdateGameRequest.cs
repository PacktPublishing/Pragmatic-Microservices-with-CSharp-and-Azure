namespace Codebreaker.GameAPIs.Client.Models;

/// <summary>
/// Set a move for a game
/// </summary>
/// <param name="Id">The unique game identifier</param>
/// <param name="GameType">The game type with one of the <see cref="GameType"/>enum values</param>
/// <param name="PlayerName"></param>
/// <param name="MoveNumber"></param>
/// <param name="End"></param>
internal record class UpdateGameRequest(
    Guid Id,
    GameType GameType,
    string PlayerName,
    int MoveNumber,
    bool End = false)
{
    public string[]? GuessPegs { get; set; }
}
