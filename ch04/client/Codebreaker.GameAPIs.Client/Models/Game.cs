namespace Codebreaker.GameAPIs.Client.Models;

public class Game(
    Guid id,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves)
{
    public Guid Id { get; private set; } = id;
    public string GameType { get; private set; } = gameType;
    public string PlayerName { get; private set; } = playerName;
    public bool PlayerIsAuthenticated { get; set; } = false;
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; } = 0;
    public int NumberCodes { get; private set; } = numberCodes;
    public int MaxMoves { get; private set; } = maxMoves;
    public bool IsVictory { get; set; } = false;

    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }

    public required string[] Codes { get; init; }
    public ICollection<Move> Moves { get; init; } = [];

    public override string ToString() => $"{Id}:{GameType} - {StartTime}";
}
