namespace Codebreaker.GameAPIs.Models;
public readonly partial struct SimpleColorResult :  ISpanFormattable
{
    public override string ToString() => ToString(default, default);

    public string ToString(string? format = default, IFormatProvider? formatProvider = default)
    {
        int length = Results.Length;
        char[] buffer = new char[(length << 1) - 1];
        if (TryFormat(buffer.AsSpan(), out int _))
        {
            return new string(buffer);
        }
        else
        {
            throw new FormatException();
        }
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        int length = Results.Length;
        if (destination.Length < ((length << 1) - 1))
        {
            charsWritten = 0;
            return false;
        }

        for (int i = 0, j = 0; i < length; i++, j += 2)
        {
            destination[j] = (char)((byte)Results[i] + '0'); 
            if (j < (length * 2 - 2)) 
                destination[j + 1] = Separator;
        }
        charsWritten = length * 2 - 1;
        return true;
    }
}
