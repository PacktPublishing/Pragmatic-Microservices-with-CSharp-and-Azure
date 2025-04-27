using System.Collections;

using static Codebreaker.GameAPIs.Models.Colors;

namespace Codebreaker.GameAPIs.Analyzer.Tests;

public class ColorGame8x5AnalyzerTests
{
    [Fact]
    public void GetResult_Should_ReturnThreeWhite()
    {
        ColorResult expectedKeyPegs = new(0, 3);
        ColorResult? resultKeyPegs = TestSkeleton(
            [Green, Yellow, Green, Black, Orange],
            [Yellow, Green, Black, Blue, Blue]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [InlineData(1, 2, Red, Yellow, Red, Blue, Orange)]
    [InlineData(2, 0, White, White, Blue, Red, White)]
    [Theory]
    public void GetResult_ShouldReturn_InlineDataResults(int expectedBlack, int expectedWhite, params string[] guessValues)
    {
        string[] code = [Red, Green, Blue, Red, Brown];
        ColorResult expectedKeyPegs = new (expectedBlack, expectedWhite);
        ColorResult resultKeyPegs = TestSkeleton(code, guessValues);
        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Theory]
    [ClassData(typeof(TestData8x5))]
    public void GetResult_ShouldReturn_InlineClassData(string[] code, string[] guess, ColorResult expectedKeyPegs)
    {
        ColorResult actualKeyPegs = TestSkeleton(code, guess);
        Assert.Equal(expectedKeyPegs, actualKeyPegs);
    }

    [Fact]
    public void GetResult_Should_ThrowOnInvalidGuessCount()
    {
        Assert.Throws<ArgumentException>(() => 
            TestSkeleton(
                ["Black", "Black", "Black", "Black", "Black"],
                ["Black"]
            ));
    }

    [Fact]
    public void GetResult_Should_ThrowOnInvalidGuessValues()
    {
        Assert.Throws<ArgumentException>(() => 
            TestSkeleton(
                ["Black", "Black", "Black", "Black", "Black"],
                ["Black", "Der", "Blue", "Yellow", "Black"]      // "Der" is the wrong value
            ));
    }

    private static ColorResult TestSkeleton(string[] codes, string[] guesses)
    {
        MockColorGame game = new()
        {
            GameType = GameTypes.Game8x5,
            NumberCodes = 5,
            MaxMoves = 14,
            IsVictory = false,
            FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                [FieldCategories.Colors] = [.. TestData8x5.Colors8]
            },
            Codes = codes
        };

        ColorGameGuessAnalyzer analyzer = new(game, [.. guesses.ToPegs<ColorField>()], 1);
        return analyzer.GetResult();
    }
}

public class TestData8x5 : IEnumerable<TheoryDataRow<string[], string[], ColorResult>>
{
    public static readonly string[] Colors8 = [Red, Blue, Green, Yellow, Black, White, Purple, Orange];

    public IEnumerator<TheoryDataRow<string[], string[], ColorResult>> GetEnumerator()
    {
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [Green, Blue,  Green, Yellow, Orange], // code
            [Green, Green, Black, White, Black],  // inputdata
            new ColorResult(1, 1) // expected
        );
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [Red, Blue, Black, White, Orange],
            [Black, Black, Red, Yellow, Yellow],
            new ColorResult(0, 2)
        );
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [Yellow, Black, Yellow, Green, Orange],
            [Black,  Black, Black,  Black, Black],
            new ColorResult(1, 0)
        );
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [Yellow, Yellow, White, Red, Orange],
            [Green,  Yellow, White, Red, Green],
            new ColorResult(3, 0)
        );
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [White, Black, Yellow, Black, Orange],
            [Black, Blue,  Black,  White, White],
            new ColorResult(0, 3)
        );
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
