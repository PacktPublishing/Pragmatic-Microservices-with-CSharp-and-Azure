namespace Codebreaker.GameAPIs.Models;

public enum ResultValue : byte
{
    Incorrect = 0,
    CorrectColor = 1,
    CorrectPositionAndColor = 2
}

// don't use record here because we want to implement equality be comparing the values of the array instead of the reference
public readonly partial struct SimpleColorResult(ResultValue[] results)
{
    private const char Separator = ':';

    public readonly ResultValue[] Results { get; } = results;
}
