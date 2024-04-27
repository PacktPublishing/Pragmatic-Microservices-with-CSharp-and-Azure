using System.Diagnostics.CodeAnalysis;

namespace CodeBreaker.Blazor.Client.Models;

internal partial class Field : IParsable<Field>
{
    public static Field Parse(string s, IFormatProvider? provider = null) =>
        s?.Split(';', StringSplitOptions.RemoveEmptyEntries) switch
        {
            [string shape, string color] => new() { Shape = shape, Color = color },
            [string color] => new() { Color = color },
            _ => throw new FormatException()
        };

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Field result)
    {
        result = s?.Split(';', StringSplitOptions.RemoveEmptyEntries) switch
        {
            [string shape, string color] => new() { Shape = shape, Color = color },
            [string color] => new () { Color = color },
            _ => null
        };

        return result is not null;
    }

    public static IEnumerable<Field> Parse(IEnumerable<string> strings, IFormatProvider? provider = null) =>
        strings.Select(s => Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] IEnumerable<string> strings, IFormatProvider? provider, [MaybeNullWhen(false)] out IEnumerable<Field> result)
    {
        List<Field> tempResult = [];

        foreach (var s in strings)
        {
            if (!TryParse(s, provider, out var partialResult))
            {
                result = null;
                return false;
            }

            tempResult.Add(partialResult);
        }

        result = tempResult;
        return true;
    }
}
