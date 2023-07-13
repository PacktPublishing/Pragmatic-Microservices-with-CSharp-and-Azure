namespace Codebreaker.GameAPIs.Client.Models;
public record class UpdateGameRequest(
    Guid GameId,
    GameType GameType,
    string PlayerName,
    int MoveNumber,
    bool End = false)
{
    public string[]? GuessPegs { get; set; }
}
