namespace Codebreaker.GameAPIs.Errors;

public record class InvalidGameMoveError(string Message)
{
    public override string ToString() => Message;
}
