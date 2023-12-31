using System.Diagnostics.CodeAnalysis;

namespace Codebreaker.GameAPIs.Models;

public readonly partial record struct ColorResult : ISpanParsable<ColorResult>
{
    public static ColorResult Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }
        else
        {
            throw new FormatException($"Cannot parse {s}");
        }
    }
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out ColorResult result)
    {
        result = s switch
        {
            { Length: > 3 or < 3 } => default,
            [var correct, Separator, var wrongpos] => new ColorResult(correct - '0', wrongpos - '0'),
            _ => default,
        };

        return result != default;
    }

    public static ColorResult Parse(string s, IFormatProvider? provider = default) =>
        Parse(s.AsSpan(), provider);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ColorResult result) =>
        TryParse(s.AsSpan(), provider, out result);
}
