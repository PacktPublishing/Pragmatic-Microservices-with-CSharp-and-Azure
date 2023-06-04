namespace Codebreaker.GameAPIs.Models;

public enum ResultValue : byte
{
    Incorrect = 0,
    CorrectColor = 1,
    CorrectPositionAndColor = 2
}

// don't use record here because we want to implement equality be comparing the values of the array instead of the reference
public readonly partial struct SimpleColorResult
{
    private const char Separator = ':';
    public SimpleColorResult(ResultValue[] results)
    {
        Results = results;
    }

    public readonly ResultValue[] Results { get; }

    public override string ToString() =>
        string.Join(Separator, Results.Select(r => (byte)r));
}
