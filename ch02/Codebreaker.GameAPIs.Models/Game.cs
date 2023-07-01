using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Models;

public abstract class Game(
    Guid gameId,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves)
{
    public Guid GameId { get; private set; } = gameId;
    public string GameType { get; private set; } = gameType;
    public string PlayerName { get; private set; } = playerName;
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; } = 0;
    public int NumberCodes { get; private set; } = numberCodes;
    public int MaxMoves { get; private set; } = maxMoves;
    public bool Won { get; set; } = false;

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}

public class Game<TField, TResult>(
    Guid gameId,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves)
    : Game(gameId, gameType, playerName, startTime, numberCodes, maxMoves),
    IGame<TField, TResult>
    where TResult: struct, IParsable<TResult>
    where TField: IParsable<TField>
{
    /// <summary>
    /// possible fields the player can choose from
    /// </summary>
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }

    /// <summary>
    /// The code to guess
    /// </summary>
    public required IEnumerable<TField> Codes { get; init; }

    public Move AddMove(TField[] guesses, TResult result, int moveNumber)
    {
        Move<TField, TResult> move = new (GameId, Guid.NewGuid(), moveNumber)
        {
            GuessPegs = guesses,
            KeyPegs = result
        };
        Moves.Add(move);
        return move;
    }

    public ICollection<Move<TField, TResult>> Moves { get; } = new List<Move<TField, TResult>>();
}
