using System.ComponentModel.DataAnnotations;

using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Models;

public class Game(
    Guid gameId,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves) : IGame
{
    [Required]
    public Guid GameId { get; private set; } = gameId;
    [Required]
    public string GameType { get; private set; } = gameType;
    [Required, MinLength(4), MaxLength(25)]
    public string PlayerName { get; private set; } = playerName;
    public bool PlayerIsAuthenticated { get; set; } = false;
    [Required]
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    [Required]
    public int LastMoveNumber { get; set; } = 0;
    [Required]
    public int NumberCodes { get; private set; } = numberCodes;
    [Required]
    public int MaxMoves { get; private set; } = maxMoves;
    [Required]
    public bool IsVictory { get; set; } = false;

    [Required]
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }

    [Required]
    public required string[] Codes { get; init; }
    [Required]
    public ICollection<Move> Moves { get; } = new List<Move>();

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}
