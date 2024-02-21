using System.Diagnostics.CodeAnalysis;

namespace Codebreaker.GameAPIs.Models;

public partial record class ShapeAndColorField : IParsable<ShapeAndColorField>
{
    public static ShapeAndColorField Parse(string s, IFormatProvider? provider = default)
    {
        if (TryParse(s, provider, out ShapeAndColorField? shape))
        {
            return shape;
        }
        else
        {
            throw new ArgumentException($"Cannot parse value {s} - use ';' to separate shape and color", nameof(s)) { HResult = 4404 };
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ShapeAndColorField result)
    {
        result = default;
        if (s is null)
        {
            return false;
        }
        string[] parts = s.Split(';');
        if (parts.Length != 2)
        {
            return false;
        }
        result = new ShapeAndColorField(parts[0], parts[1]);
        return true;
    }
}
