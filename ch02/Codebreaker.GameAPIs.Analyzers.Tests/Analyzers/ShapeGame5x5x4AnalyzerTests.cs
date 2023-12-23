using System.Collections;

using static Codebreaker.GameAPIs.Models.Colors;
using static Codebreaker.GameAPIs.Models.Shapes;

namespace Codebreaker.GameAPIs.Analyzer.Tests;

public class ShapeGame5x5x4AnalyzerTests
{


    [Fact]
    public void SetMoveShouldReturnTwoBlack()
    {
        ShapeAndColorResult expectedKeyPegs = new(2, 0, 0);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Rectangle;Green", "Circle;Yellow"],
            ["Rectangle;Green", "Circle;Yellow", "Star;Blue", "Star;Blue"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnOneBlackWithMultipleCorrectCodes()
    {
        ShapeAndColorResult expectedKeyPegs = new(1, 0, 0);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Rectangle;Green", "Rectangle;Green", "Rectangle;Green"],
            ["Rectangle;Green", "Star;Blue", "Star;Blue", "Star;Blue"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnOneBlackWithMultipleCorrectPairGuesses()
    {
        ShapeAndColorResult expectedKeyPegs = new(1, 0, 0);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Circle;Yellow", "Circle;Yellow"],
            ["Rectangle;Green", "Rectangle;Green", "Rectangle;Green", "Rectangle;Green"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnThreeWhite()
    {
        ShapeAndColorResult expectedKeyPegs = new(0, 3, 0);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Rectangle;Green", "Star;Blue"],
            ["Circle;Yellow", "Rectangle;Green", "Star;Blue", "Square;Purple"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnOneWhiteWithMultipleCorrectPairIsGuesses()
    {
        ShapeAndColorResult expectedKeyPegs = new(0, 1, 0);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Circle;Yellow", "Circle;Yellow"],
            ["Triangle;Blue", "Rectangle;Green", "Rectangle;Green", "Rectangle;Green"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnTwoBlueForMatchingColors()
    {
        // the second and third guess have a correct color in the correct position
        // all the shapes are incorrect
        ShapeAndColorResult expectedKeyPegs = new(0, 0, 2);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Rectangle;Green", "Circle;Yellow"],
            ["Star;Blue", "Star;Yellow", "Star;Green", "Star;Blue"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnTwoBlueForMatchingShapesAndColors()
    {
        // the first guess has a correct shape, and the second guess a correct color. All other guesses are wrong.
        ShapeAndColorResult expectedKeyPegs = new(0, 0, 2);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Rectangle;Green", "Circle;Yellow"],
            ["Rectangle;Blue", "Rectangle;Yellow", "Star;Blue", "Star;Blue"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnTwoBlueForMatchingShapes()
    {
        // the first and second guess have a correct shape, but a wrong color
        // all the colors are incorrect
        ShapeAndColorResult expectedKeyPegs = new(0, 0, 2);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Green", "Circle;Yellow", "Rectangle;Green", "Circle;Yellow"],
            ["Rectangle;Blue", "Circle;Blue", "Star;Blue", "Star;Blue"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    [Fact]
    public void SetMoveShouldReturnOneBlackAndOneWhite()
    {
        // the first and second guess have a correct shape, but both in the wrong positon
        // all the colors are incorrect
        ShapeAndColorResult expectedKeyPegs = new(1, 1, 0);
        ShapeAndColorResult? resultKeyPegs = TestSkeleton(
            ["Rectangle;Blue", "Circle;Yellow", "Star;Green", "Circle;Yellow"],
            ["Rectangle;Blue", "Star;Green", "Triangle;Red", "Triangle;Red"]
        );

        Assert.Equal(expectedKeyPegs, resultKeyPegs);
    }

    private static ShapeAndColorResult TestSkeleton(string[] codes, string[] guesses)
    {
        MockShapeGame game = new()
        {
            GameType = GameTypes.Game5x5x4,
            NumberCodes = 4,
            MaxMoves = 14,
            IsVictory = false,
            FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                [FieldCategories.Colors] = TestData5x5x4.Colors5.ToList(),
                [FieldCategories.Shapes] = TestData5x5x4.Shapes5.ToList()
            },
            Codes = codes
        };

        ShapeGameGuessAnalyzer analyzer = new(game, guesses.ToPegs<ShapeAndColorField>().ToArray(), 1);
        return analyzer.GetResult();
    }
}

public class TestData5x5x4 : IEnumerable<object[]>
{
    public static readonly string[] Colors5 = [Red, Green, Blue, Yellow, Purple];
    public static readonly string[] Shapes5 = [Circle, Square, Triangle, Star, Rectangle];

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new string[] { Green, Blue,  Green, Yellow }, // code
            new string[] { Green, Green, Black, White },  // inputdata
            new ColorResult(1, 1) // expected
        };
        yield return new object[]
        {
            new string[] { Red,   Blue,  Black, White },
            new string[] { Black, Black, Red,   Yellow },
            new ColorResult(0, 2)
        };
        yield return new object[]
        {
            new string[] { Yellow, Black, Yellow, Green },
            new string[] { Black,  Black, Black,  Black },
            new ColorResult(1, 0)
        };
        yield return new object[]
        {
            new string[] { Yellow, Yellow, White, Red },
            new string[] { Green,  Yellow, White, Red },
            new ColorResult(3, 0)
        };
        yield return new object[]
        {
            new string[] { White, Black, Yellow, Black },
            new string[] { Black, Blue,  Black,  White },
            new ColorResult(0, 3)
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
