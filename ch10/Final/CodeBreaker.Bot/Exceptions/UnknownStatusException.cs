namespace CodeBreaker.Bot.Exceptions;

public class UnknownStatusException : Exception
{
    public UnknownStatusException()
    {
    }

    public UnknownStatusException(string? message) : base(message)
    {
    }
}
