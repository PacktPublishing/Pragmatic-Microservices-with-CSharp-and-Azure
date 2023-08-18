namespace Codebreaker.GameAPIs.Models;

public readonly partial record struct ShapeAndColorResult(byte Correct, byte WrongPosition, byte ColorOrShape)
{
    private const char Separator = ':';
}
