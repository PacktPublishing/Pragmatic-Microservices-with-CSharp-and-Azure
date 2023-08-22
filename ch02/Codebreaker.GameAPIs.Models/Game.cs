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
    public Guid GameId { get; } = gameId;
    public string GameType { get; } = gameType;
    public string PlayerName { get; } = playerName;
    public DateTime StartTime { get; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; } = 0;
    public int NumberCodes { get; private set; } = numberCodes;
    public int MaxMoves { get; private set; } = maxMoves;
    public bool IsVictory { get; set; } = false;

    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }

    public required string[] Codes { get; init; }
    public ICollection<Move> Moves { get; } = new List<Move>();

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}
