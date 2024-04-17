using System.Runtime.CompilerServices;

namespace CodeBreaker.Bot;

public record struct KeyPegWithFlag(int Value, bool Used);

public static class CodeBreakerAlgorithms
{
    // definitions to mask the different pegs
    private const int C0001 = 0b_111111;
    private const int C0010 = 0b_111111_000000;
    private const int C0100 = 0b_111111_000000_000000;
    private const int C1000 = 0b_111111_000000_000000_000000;

    // Convert the int representation of four pegs to an array of CodeColors names
    public static string[] IntToColors(this int value)
    {
        int i1 = (value >> 0) & 0b111111;
        int i2 = (value >> 6) & 0b111111;
        int i3 = (value >> 12) & 0b111111;
        int i4 = (value >> 18) & 0b111111;
        var c1 = (CodeColors)i1;
        var c2 = (CodeColors)i2;
        var c3 = (CodeColors)i3;
        var c4 = (CodeColors)i4;
        string[] colorNames =
        [
            c4.ToString(), c3.ToString(), c2.ToString(), c1.ToString()
        ];

        return colorNames;
    }

    /// <summary>
    /// Reduces the possible values based on the black matches with the selection
    /// </summary>
    /// <param name="values">The list of possible moves</param>
    /// <param name="blackHits">The number of black hits with the selection</param>
    /// <param name="selection">The key pegs of the selected move</param>
    /// <returns>The remaining possible moves</returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<int> HandleBlackMatches(this IList<int> values, int blackHits, int selection)
    {
        if (blackHits is < 1 or > 3)
        {
            throw new ArgumentException("invalid argument - hits need to be between 1 and 3");
        }

        static bool IsMatch(int value, int blackhits, int selection)
        {
            int n1 = selection & C0001;
            int n2 = selection & C0010;
            int n3 = selection & C0100;
            int n4 = selection & C1000;
            int matches = 0;
            bool match1 = (value & n1) == n1;
            if (match1) 
                matches++;
            if ((value & n2) == n2) 
                matches++;
            if ((value & n3) == n3) 
                matches++;
            if ((value & n4) == n4) 
                matches++;
            return (matches == blackhits);
        }

        List<int> result = new(capacity: values.Count);

        foreach (int value in values)
        {
            if (IsMatch(value, blackHits, selection))
            {
                result.Add(value);
            }
        }
        return result;
    }

    /// <summary>
    /// Reduces the possible values based on the white matches with the selection
    /// </summary>
    /// <param name="values">The possible values</param>
    /// <param name="whiteHits">The number of white hits with the selection</param>
    /// <param name="selection">The selected pegs</param>
    /// <returns>The remaining possbile values</returns>
    public static List<int> HandleWhiteMatches(this IList<int> values, int whiteHits, int selection)
    {
        List<int> newValues = new(values.Count);
        foreach (int value in values)
        {
            // need to have clean selections with every run
            KeyPegArray selections = new();
            for (int i = 0; i < 4; i++)
            {
                selections[i] = new KeyPegWithFlag(selection.SelectPeg(i), false);
            }

            KeyPegArray matches = new();
            for (int i = 0; i < 4; i++)
            {
                matches[i] = new KeyPegWithFlag(value.SelectPeg(i), false);
            }

            int matchCount = 0;
            for (int i = 0; i < 4; i++)
            {

                for (int j = 0; j < 4; j++)
                {
                    if (!matches[i].Used && !selections[j].Used && matches[i].Value == selections[j].Value)
                    {
                        matchCount++;
                        selections[j] = selections[j] with { Used = true };
                        matches[i] = matches[i] with { Used = true };
                    }
                }
            }
            if (matchCount == whiteHits)
            {
                newValues.Add(value);
            }
        }

        return newValues;
    }

    /// <summary>
    /// Reduces the possible values if no selection was correct
    /// </summary>
    /// <param name="values">The possible values</param>
    /// <param name="selection">The selected pegs</param>
    /// <returns>The remaining possbile values</returns>
    public static List<int> HandleNoMatches(this IList<int> values, int selection)
    {
        static bool Contains(int[] selections, int value)
        {
            for (int i = 0; i < 4; i++)
            {
                if (selections.Contains(value.SelectPeg(i)))
                {
                    return true;
                }
            }
            return false;
        }

        List<int> newValues = new(values.Count);
        int[] selections = Enumerable.Range(0, 4)
            .Select(i => selection.SelectPeg(i))
            .ToArray();

        foreach (int value in values)
        {
            if (!Contains(selections, value))
            {
                newValues.Add(value);
            }
        }
        return newValues;
    }

    /// <summary>
    /// Get the int representation of one peg from the int representaiton of four pegs
    /// </summary>
    /// <param name="code">The int value representing four pegs</param>
    /// <param name="pegNumber">The peg number to retrieve from the int representation</param>
    /// <returns>The int value of the selected peg</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static int SelectPeg(this int code, int pegNumber) => 
        pegNumber switch
        {
            0 => code & 0b111111,
            1 => (code >> 6) & 0b111111,
            2 => (code >> 12) & 0b111111,
            3 => (code >> 18) & 0b111111,
            _ => throw new InvalidOperationException("invalid peg number")
        };
}

[InlineArray(4)]
internal struct KeyPegArray
{
#pragma warning disable IDE0051 // Remove unused private members
    private KeyPegWithFlag _keyPegWithFlag;
#pragma warning restore IDE0051 // Remove unused private members
}