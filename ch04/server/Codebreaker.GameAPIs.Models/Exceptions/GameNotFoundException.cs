namespace Codebreaker.GameAPIs.Exceptions;

public class GameNotFoundException : Exception
{
    public GameNotFoundException() { }
    public GameNotFoundException(string message) : base(message) { }
    public GameNotFoundException(string message, Exception inner) : base(message, inner) { }
}
