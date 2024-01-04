namespace Codebreaker.GameAPIs.Models;

public readonly partial record struct ColorResult(int Correct, int WrongPosition)
{
    private const char Separator = ':';
}
