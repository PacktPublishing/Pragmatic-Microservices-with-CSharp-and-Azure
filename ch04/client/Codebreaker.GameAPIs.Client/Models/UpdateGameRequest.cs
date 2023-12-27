namespace Codebreaker.GameAPIs.Client.Models;

public record class UpdateGameRequest(
    Guid Id,
    GameType GameType,
    string PlayerName,
    int MoveNumber,
    bool End = false)
{
    public string[]? GuessPegs { get; set; }
}
