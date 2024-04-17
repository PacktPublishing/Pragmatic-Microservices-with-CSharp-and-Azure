using System.Collections;

using Xunit;

namespace CodeBreaker.Bot.Tests;

public class CodeBreakerAlgorithmsTests
{
    [Fact]
    public void SelectPeg_Should_ThrowException()
    {
        Assert.Throws<InvalidOperationException>(() => 
            CodeBreakerAlgorithms.SelectPeg(44, 4));
    }

    [Theory]
    [InlineData(0b_000100_000100_000100_000100, 0, 0b_000100)]
    [InlineData(0b_000100_000100_000100_000100, 1, 0b_000100)]
    [InlineData(0b_000100_000100_000100_000100, 2, 0b_000100)]
    [InlineData(0b_000100_000100_000100_000100, 3, 0b_000100)]
    public void SelectPegTest(int code, int number, int expected)
    {
        int actual = CodeBreakerAlgorithms.SelectPeg(code, number);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HandleBlackMatches_Should_Find1BlackMatch()
    {
        List<int> toMatch =
        [
            0b_100000_010000_100000_100000,  // hit
            0b_100000_010000_010000_100000,  // hit
            0b_000100_000100_000100_000100   // miss
        ];
        int selection = 0b_000001_010000_000001_000001;

        List<int> actual = CodeBreakerAlgorithms.HandleBlackMatches(toMatch, 1, selection);
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public void HandleBlackMatches_Should_Find2BlackMatches()
    {
        List<int> toMatch =
        [
            0b_100000_010000_010000_100000,  // hit
            0b_000001_100000_010000_100000,  // hit
            0b_000100_010000_000100_000100   // miss
        ];
        int selection = 0b_000001_010000_010000_000001;

        List<int> actual = CodeBreakerAlgorithms.HandleBlackMatches(toMatch, 2, selection);
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public void HandleBlackMatches_Should_Find3BlackMatches()
    {
        List<int> toMatch =
        [
            0b_100000_010000_010000_100000,  // miss
            0b_000001_100000_010000_100000,  // hit
            0b_000100_010000_000100_000100   // miss
        ];
        int selection = 0b_000001_100000_010000_000001;

        List<int> actual = CodeBreakerAlgorithms.HandleBlackMatches(toMatch, 3, selection);
        Assert.Single(actual);
    }

    [Fact]
    public void HandleBlackMatches_Should_BeEmpty()
    {
        List<int> toMatch =
        [
            0b_000100_010000_001000_000010
        ];
        int selection = 0b_000001_010000_001000_001000;
        List<int> actual = CodeBreakerAlgorithms.HandleBlackMatches(toMatch, 1, selection);
        Assert.Empty(actual);
    }

    [Fact]
    public void HandleWhiteMatches_Should_Find1WhiteMatches()
    {
        List<int> toMatch =
        [
            0b_010000_100000_100000_100000,  // hit
            0b_010000_100000_010000_100000,  // hit
            0b_000100_000100_000100_000100   // miss
        ];
        int selection = 0b_000001_010000_000001_000001;

        List<int> actual = CodeBreakerAlgorithms.HandleWhiteMatches(toMatch, 1, selection);
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public void IntToColors_Should_ConvertToCorrectColor()
    {
        int value = 0b_000100_010000_000001_100000;
        string[] expected = ["Red", "Blue", "Black", "Yellow"];
        string[] actual = CodeBreakerAlgorithms.IntToColors(value);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HandleNoMatches_Should_MatchOneResult()
    {
        List<int> toMatch =
        [
            0b_010000_100000_100000_100000,  // miss
            0b_010000_100000_010000_100000,  // miss
            0b_001000_001000_001000_001000   // hit
        ];
        int selection = 0b_000100_010000_000001_100000;
        List<int> actual = CodeBreakerAlgorithms.HandleNoMatches(toMatch, selection);
        Assert.Single(actual);
    }

    //private IEnumerable<string> _values;
    //public MMAlgorithmsTests()
    //{
    //    _values = new List<string>()
    //    {
    //        "0123", "4512", "5555", "4444", "3423"
    //    };
    //}

    //[Theory]
    //[InlineData("0123", "black", "white", "red", "green")]
    //[InlineData("4501", "blue", "yellow", "black", "white")]
    //public void TestColorNamesStringToColor(string input, params string[] colors)
    //{
    //    (var colorNames, var chars) = MMAlgorithms.StringToColors(input);
    //    Assert.Equal(colors, colorNames);
    //}

    //[Theory]
    //[InlineData("0123", '0', '1', '2', '3')]
    //[InlineData("4501", '4', '5', '0', '1')]
    //public void TestColorNamesStringToNumbers(string input, params char[] charsexpected)
    //{
    //    (var colorNames, var chars) = MMAlgorithms.StringToColors(input);
    //    Assert.Equal(charsexpected, chars);
    //}

    //[Theory]
    //[InlineData(1, 1, '0', '0', '0', '0')]
    //[InlineData(1, 2, '4', '4', '4', '4')]
    //[InlineData(2, 1, '2', '3', '2', '3')]
    //public void TestReducePossibleValues(int hits, int expectedCount, params char[] chars)
    //{
    //    List<string> results = MMAlgorithms.ReducePossibleValues(_values, hits, chars);
    //    Assert.Equal(expectedCount, results.Count);
    //}

    //[Theory]
    //[ClassData(typeof(TestReduceData))]
    //public void TestReducePossibleValuesReturnsHit(string code, int hits, char[] chars)
    //{
    //    List<string> values = new() { code };
    //    var actual = MMAlgorithms.ReducePossibleValues(values, hits, chars);
    //    Assert.True(actual.Count() == 1);
    //}
}

public class TestReduceData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "4245",
            3,
            new char[] { '4', '5', '4', '1' }
        };
        yield return new object[]
        {
            "2454",
            1,
            new char[] { '1', '4', '3', '1' }
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
