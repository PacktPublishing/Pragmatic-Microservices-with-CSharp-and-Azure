namespace Codebreaker.GameAPIs.Exceptions;

public class GameTypeNotFoundException : Exception
{
    public GameTypeNotFoundException() { }
    public GameTypeNotFoundException(string message) : base(message) { }
    public GameTypeNotFoundException(string message, Exception inner) : base(message, inner) { }
}
