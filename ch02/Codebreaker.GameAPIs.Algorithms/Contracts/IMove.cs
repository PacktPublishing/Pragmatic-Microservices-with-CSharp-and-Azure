namespace Codebreaker.GameAPIs.Contracts;

public interface IMove<TField, TResult>
    where TResult: struct
{
    int MoveNumber { get; }
    ICollection<TField> GuessPegs { get; }
    TResult? KeyPegs { get; }
}
