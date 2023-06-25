using Codebreaker.GameAPIs.Models;

namespace Codebreaker.Data.SqlServer.Tests;

public class MappingExtensionsTests
{
    [Fact]
    public void TestToFieldCollection()
    {
        string input = "Red#Green#Blue";
        ColorField[] expected = { "Red", "Green", "Blue" };
        ICollection<ColorField> actual = input.ToFieldCollection<ColorField>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestToFieldString()
    {
        ColorField[] fields = { "Red", "Green", "Blue" };
        string expected = "Red#Green#Blue";
        string actual = fields.ToFieldString();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToLookupString_ShouldReturnCorrectLookupString()
    {
        // Arrange
        var lookup = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("key1", "item1"),
                new KeyValuePair<string, string>("key1", "item2"),
                new KeyValuePair<string, string>("key2", "item3"),
                new KeyValuePair<string, string>("key2", "item4")
            }.ToLookup(pair => pair.Key, pair => pair.Value);

        var expected = "key1:item1#key1:item2#key2:item3#key2:item4";

        // Act
        var result = MappingExtensions.ToLookupString(lookup);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToLookup_ShouldReturnCorrectLookupObject()
    {
        // Arrange
        var input = "key1:item1#key1:item2#key2:item3#key2:item4";
        var expected = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("key1", "item1"),
                new KeyValuePair<string, string>("key1", "item2"),
                new KeyValuePair<string, string>("key2", "item3"),
                new KeyValuePair<string, string>("key2", "item4")
            }.ToLookup(pair => pair.Key, pair => pair.Value);

        // Act
        var result = MappingExtensions.ToLookup(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
