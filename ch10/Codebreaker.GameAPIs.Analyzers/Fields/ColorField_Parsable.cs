using System.Diagnostics.CodeAnalysis;

namespace Codebreaker.GameAPIs.Models;

public partial record class ColorField : IParsable<ColorField>
{
    public static ColorField Parse(string s, IFormatProvider? provider = default)
    {
        if (TryParse(s, provider, out ColorField? color))
        {
            return color;
        }
        else
        {
            throw new ArgumentException($"Cannot parse value {s}") { HResult = 4404 };
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ColorField result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }
        result = new ColorField(s);
        return true;
    }
}
