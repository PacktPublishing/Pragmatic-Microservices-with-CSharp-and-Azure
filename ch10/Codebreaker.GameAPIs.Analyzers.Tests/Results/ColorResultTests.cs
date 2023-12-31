namespace Codebreaker.GameAPIs.Analyzer.Tests;

public class ColorResultTests
{
    [Fact]
    public void ToStringShouldReturnFormat()
    {
        string expected = "1:2";
        ColorResult colorResult = new(Correct: 1, WrongPosition: 2);
        string actual = colorResult.ToString();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParseShouldReturnColorResult()
    {
        string input = "1:2";
        ColorResult expected = new(1, 2);
        var actual = ColorResult.Parse(input);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFormatShouldReturnFalseWithSmallSpan()
    {
        bool expected = false;
        char[] chars = new char[2];
        bool actual = ColorResult.TryParse(chars.AsSpan(), null, out _);
        Assert.Equal(expected, actual);
    }
}