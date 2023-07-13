namespace Codebreaker.GameAPIs.Client.Models;
public record class UpdateGameResponse(
    Guid GameId,
    GameType GameType,
    int MoveNumber,
    bool Ended,
    bool IsVictory,
    string[] Results);
