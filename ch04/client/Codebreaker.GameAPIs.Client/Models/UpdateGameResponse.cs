namespace Codebreaker.GameAPIs.Client.Models;

public record class UpdateGameResponse(
    Guid Id,
    GameType GameType,
    int MoveNumber,
    bool Ended,
    bool IsVictory,
    string[] Results);
