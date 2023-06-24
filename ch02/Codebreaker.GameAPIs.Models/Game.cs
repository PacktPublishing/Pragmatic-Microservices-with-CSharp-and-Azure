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
    public Guid GameId { get; } = gameId;
    public string GameType { get; } = gameType;
    public string PlayerName { get; } = playerName;
    public DateTime StartTime { get; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; } = 0;
    public int NumberCodes { get; private set; } = numberCodes;
    public int MaxMoves { get; } = maxMoves;
    public bool Won { get; set; } = false;

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}

public class Game<TField, TResult>(
    Guid gameId,
    string gameType,
    string playerName,
    DateTime startTime,
    int holes,
    int maxMoves)
    : Game(gameId, gameType, playerName, startTime, holes, maxMoves),
    IGame<TField, TResult>
    where TResult: struct, IParsable<TResult>
    where TField: IParsable<TField>
{
    // possible fields the player can choose from
    public required ILookup<string, string> FieldValues { get; init; }

    // the code to guess
    public required TField[] Codes { get; init; }

    public void AddMove(TField[] guesses, TResult result, int moveNumber)
    {
        Move<TField, TResult> move = new (GameId, Guid.NewGuid(), moveNumber)
        {
            GuessPegs = guesses,
            KeyPegs = result
        };
        Moves.Add(move);
    }

    //public Move<TField, TResult> CreateMove(TField[] fields, TResult result, int moveNumber) => 
    //    new Move<TField, TResult>(GameId, Guid.NewGuid(), moveNumber)
    //{
    //    GuessPegs = fields,
    //    KeyPegs = result
    //};

    public ICollection<Move<TField, TResult>> Moves { get; } = new List<Move<TField, TResult>>();
}
