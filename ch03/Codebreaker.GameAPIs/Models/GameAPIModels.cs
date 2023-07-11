namespace Codebreaker.GameAPIs.Models;

public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4
}

public record class CreateGameRequest(GameType GameType, string PlayerName);

public record class CreateGameResponse(Guid GameId, GameType GameType, string PlayerName)
{
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }
}

public record class SetMoveRequest(Guid GameId, GameType GameType, string PlayerName, int MoveNumber)
{
    public required string[] GuessPegs { get; set; }
}

public record SetMoveResponse(
    Guid GameId,
    GameType GameType,
    int MoveNumber,
    bool Ended,
    bool IsVictory,
    string[] Results);

public record GameSummary(Guid GameId, string PlayerName, DateTime StartTime, int NumberMoves, bool IsVictory, TimeSpan? Duration);

public record GetGamesRankResponse(DateOnly Date, GameType GameType)
{
    public required IEnumerable<GameSummary> Games { get; set; }
}
