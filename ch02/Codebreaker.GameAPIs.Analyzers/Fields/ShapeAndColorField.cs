namespace Codebreaker.GameAPIs.Models;

public partial record class ShapeAndColorField(string Shape, string Color)
{
    private const char Separator = ';';
    public override string ToString() => $"{Shape};{Color}";

    public static implicit operator ShapeAndColorField((string Shape, string Color) f) => new(f.Shape, f.Color);
}
