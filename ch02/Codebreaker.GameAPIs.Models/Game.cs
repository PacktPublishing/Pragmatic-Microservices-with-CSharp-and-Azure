using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Models;

public abstract class Game(
    Guid gameId,
    string gameType,
    string playerName,
    bool isAuthenticated,
    DateTime startTime,
    int holes,
    int maxMoves)
    : IFormattable
{
    public Guid GameId { get; private set; } = gameId;
    public string GameType { get; private set; } = gameType;
    public string PlayerName { get; private set; } = playerName;
    public bool IsAuthenticated { get; private set; } = isAuthenticated;
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; } = 0;
    public int Holes { get; private set; } = holes;
    public int MaxMoves { get; private set; } = maxMoves;
    public bool Won { get; set; } = false;

    public string ToString(string? format = default, IFormatProvider? formatProvider = default) =>
        $"{GameId}:{GameType} - {StartTime}";

    public override string ToString() => ToString(null, null);
}

public class Game<TField, TResult>(
    Guid gameId,
    string gameType,
    string playerName,
    bool isAuthenticated,
    DateTime startTime,
    int holes,
    int maxMoves)
    : Game(gameId, gameType, playerName, isAuthenticated, startTime, holes, maxMoves),
    IGame<TField, TResult>
    where TResult: struct, IParsable<TResult>
    where TField: IParsable<TField>
{
    // possible fields the player can choose from
    public required IEnumerable<TField> Fields { get; init; }

    // the code to guess
    public required ICollection<TField> Codes { get; init; }

    public IMove<TField, TResult> CreateMove(IEnumerable<TField> fields, TResult result, int moveNumber)
    {
        return new Move<TField, TResult>(GameId, Guid.NewGuid(), moveNumber)
        {
            GuessPegs = fields.ToArray(),
            KeyPegs = result
        };
    }

    public ICollection<IMove<TField, TResult>> Moves { get; } = new List<IMove<TField, TResult>>();
}
