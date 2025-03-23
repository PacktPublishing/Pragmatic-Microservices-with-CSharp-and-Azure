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
}
