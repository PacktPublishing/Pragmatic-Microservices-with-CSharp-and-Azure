namespace Codebreaker.GameAPIs.Models;

public readonly partial record struct ColorResult : ISpanFormattable
{
    public override string ToString() => ToString(default, default);

    public string ToString(string? format = default, IFormatProvider? formatProvider = default)
    {
        var destination = new char[3].AsSpan();
        if (TryFormat(destination, out _, format.AsSpan(), formatProvider))
        {
            return destination.ToString();
        }
        else
        {
            throw new FormatException();
        }
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
    {
        if (destination.Length < 3)
        {
            charsWritten = 0;
            return false;
        }

        destination[0] = (char)(Correct + '0');
        destination[1] = Separator;
        destination[2] = (char)(WrongPosition + '0');
        charsWritten = 3;
        return true;
    }
}
