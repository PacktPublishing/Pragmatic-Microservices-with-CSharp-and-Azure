using System.Diagnostics.CodeAnalysis;

namespace Codebreaker.GameAPIs.Models;

public readonly partial struct SimpleColorResult : ISpanParsable<SimpleColorResult>
{
    public static SimpleColorResult Parse(ReadOnlySpan<char> s, IFormatProvider? provider = default)
    {
        if (TryParse(s, provider, out SimpleColorResult result))
        {
            return result;
        }
        else
        {
            throw new FormatException($"Cannot parse {s}");
        }
    }

    public static SimpleColorResult Parse(string s, IFormatProvider? provider = default)
    {
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out SimpleColorResult result)
    {
        if (s.Length < 7)
        {
            result = default;
            return false;
        }

        var values = new  ResultValue[4];
        for (int i = 0, j = 0; i < 4; i++, j += 2)
        {

            values[i] = (ResultValue)(s[j] - '0');
        }
        result = new SimpleColorResult(values);
        return s != default;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SimpleColorResult result) =>
        TryParse(s.AsSpan(), provider, out result);
}
