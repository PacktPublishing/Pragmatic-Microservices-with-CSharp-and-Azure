using Codebreaker.GameAPIs.Models;

namespace Codebreaker.Data.SqlServer.Tests;

public class MappingExtensionsTests
{
    [Fact]
    public void TestToFieldCollection()
    {
        string input = "Red#Green#Blue";
        ColorField[] expected = ["Red", "Green", "Blue"];
        ICollection<ColorField> actual = input.ToFieldCollection<ColorField>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestToFieldString()
    {
        ColorField[] fields = ["Red", "Green", "Blue"];
        string expected = "Red#Green#Blue";
        string actual = fields.ToFieldString();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToFieldString_ShouldReturnCorrectString()
    {
        // Arrange
        IDictionary<string, IEnumerable<string>> dict = new Dictionary<string, IEnumerable<string>>()
            {
                { "colors", ["Red", "Green", "Blue"] },
                { "shapes", ["Rectangle", "Circle"] }
            };

        string expected = "colors:Red#colors:Green#colors:Blue#shapes:Rectangle#shapes:Circle";

        // Act
        string result = MappingExtensions.ToFieldsString(dict);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToFieldsDictionary_ShouldReturnCorrectDictionary()
    {
        // Arrange
        string input = "colors:Red#colors:Green#colors:Blue#shapes:Rectangle#shapes:Circle";
        Dictionary<string, IEnumerable<string>> expected = new ()
            {
                { "colors", ["Red", "Green", "Blue"] },
                { "shapes", ["Rectangle", "Circle"] }
            };

        // Act
        var result = MappingExtensions.FromFieldsString(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
