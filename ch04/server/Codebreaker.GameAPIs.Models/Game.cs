using Codebreaker.GameAPIs.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Codebreaker.GameAPIs.Models;

public class Game(
    Guid id,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves) : IGame
{
    [Required]
    public Guid Id { get; private set; } = id;
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
    public ICollection<Move> Moves { get; } = [];

    public override string ToString() => $"{Id}:{GameType} - {StartTime}";
}
