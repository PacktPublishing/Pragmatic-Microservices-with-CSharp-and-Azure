namespace Codebreaker.GameAPIs.Errors;

public record InvalidGameTypeError(string Message, string[] GameTypes)
{
    public override string ToString() => $"{Message}, these game types are available: {string.Join(", ", GameTypes)}";
}
