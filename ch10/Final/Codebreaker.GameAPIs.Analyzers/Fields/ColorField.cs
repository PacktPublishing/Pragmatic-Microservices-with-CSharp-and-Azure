namespace Codebreaker.GameAPIs.Models;

public partial record class ColorField(string Color)
{
    public override string ToString() => Color;

    public static implicit operator ColorField(string color) => new(color);
}
