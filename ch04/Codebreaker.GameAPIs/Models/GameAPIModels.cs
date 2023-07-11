using System.ComponentModel.DataAnnotations;

namespace Codebreaker.GameAPIs.Models;

public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4
}

public record class CreateGameRequest(
    [property: Required] GameType GameType, 
    [property: Required, MinLength(4), MaxLength(60)] string PlayerName);

public record class CreateGameResponse(
    [property: Required] Guid GameId, 
    [property: Required] GameType GameType, 
    [property: Required, MinLength(4), MaxLength(60)] string PlayerName)
{
    [Required] 
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }
}

public record class SetMoveRequest(
    [property: Required] Guid GameId, 
    [property: Required] GameType GameType, 
    [property: Required, MinLength(4), MaxLength(60)] string PlayerName, 
    [property: Required] int MoveNumber)
{
    [Required]
    public required string[] GuessPegs { get; set; }
}

public record SetMoveResponse(
    [property: Required] Guid GameId,
    [property: Required] GameType GameType,
    [property: Required] int MoveNumber,
    [property: Required] bool Ended,
    [property: Required] bool IsVictory,
    [property: Required] string[] Results);

public record GameSummary(
    [property: Required] Guid GameId,
    [property: Required, MinLength(4), MaxLength(60)] string PlayerName,
    [property: Required] DateTime StartTime,
    [property: Required] int NumberMoves,
    [property: Required] bool IsVictory,
    TimeSpan? Duration);

public record GetGamesRankResponse(
    [property: Required] DateOnly Date, 
    [property: Required] GameType GameType)
{
    public required IEnumerable<GameSummary> Games { get; set; }
}
