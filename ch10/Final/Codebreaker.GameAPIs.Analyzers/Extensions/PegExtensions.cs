namespace Codebreaker.GameAPIs.Extensions;

public static class PegExtensions
{
    public static IEnumerable<T> ToPegs<T>(this string[] pegStrings)
        where T : IParsable<T>
    {
        foreach (string pegString in pegStrings)
        {
            yield return T.Parse(pegString, default);
        }
    }

    public static IEnumerable<string> AsStringFields<T>(this T[] pegs)
        where T : IFormattable
    {
        foreach(var peg in pegs)
        {
            yield return peg.ToString(default, default);
        }
    }

    public static string[] ToStringResults(this ColorResult result)
    {
        return Enumerable.Repeat(Colors.Black, result.Correct)
            .Concat(Enumerable.Repeat(Colors.White, result.WrongPosition))
            .ToArray();
    }

    public static string[] ToStringResults(this ShapeAndColorResult result)
    {
        return Enumerable.Repeat(Colors.Black, result.Correct)
            .Concat(Enumerable.Repeat(Colors.Blue, result.ColorOrShape))
            .Concat(Enumerable.Repeat(Colors.White, result.WrongPosition))
            .ToArray();
    }

    public static string[] ToStringResults(this SimpleColorResult result)
    {
        return result.Results.Select(val => val switch
        {
            ResultValue.CorrectPositionAndColor => Colors.Black,
            ResultValue.CorrectColor => Colors.White,
            ResultValue.Incorrect => Colors.None,
            _ => throw new ArgumentException($"The value {val} is not supported.")
        }).ToArray();
    }
}
