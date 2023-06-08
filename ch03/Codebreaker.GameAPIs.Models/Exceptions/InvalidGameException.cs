namespace Codebreaker.GameAPIs.Exceptions;

public class InvalidGameException : Exception
{
    public InvalidGameException() { }
    public InvalidGameException(string message) : base(message) { }
    public InvalidGameException(string message, Exception inner) : base(message, inner) { }

}
