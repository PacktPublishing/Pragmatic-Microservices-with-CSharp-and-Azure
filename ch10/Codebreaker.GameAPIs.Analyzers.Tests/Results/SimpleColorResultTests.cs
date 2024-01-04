namespace Codebreaker.GameAPIs.Analyzer.Tests;

public class SimpleColorResultTests
{
    [Fact]
    public void ToStringShouldReturnFormat()
    {
        ResultValue[] values = [ResultValue.CorrectColor, ResultValue.CorrectPositionAndColor, ResultValue.Incorrect, ResultValue.Incorrect];
        SimpleColorResult result = new(values);
        string actual = result.ToString();
        Assert.Equal("1:2:0:0", actual);
    }

    [Fact]
    public void ParseShouldReturnColorResult()
    {
        ResultValue[] values = [ResultValue.CorrectColor, ResultValue.CorrectPositionAndColor, ResultValue.Incorrect, ResultValue.Incorrect];
        SimpleColorResult expected = new(values);
        var actual = SimpleColorResult.Parse("1:2:0:0");
        Assert.Equal(expected, actual);
    }
}