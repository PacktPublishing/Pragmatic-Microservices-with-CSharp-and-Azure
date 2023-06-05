namespace Codebreaker.GameAPIs.Contracts;

public interface IGame<TField, TResult>
    where TResult : struct
{
    string GameType { get; }
    int Holes { get; }
    int MaxMoves { get; }
    DateTime StartTime { get; }
    DateTime? EndTime { get; set; }
    TimeSpan? Duration { get; set; }
    bool Won { get; set; }
    int LastMoveNumber { get; set; }
    IEnumerable<TField> Fields { get; }
    ICollection<TField> Codes { get; }
    ICollection<IMove<TField, TResult>> Moves { get; }
    IMove<TField, TResult> CreateMove(IEnumerable<TField> fields, TResult result, int moveNumber);
}
