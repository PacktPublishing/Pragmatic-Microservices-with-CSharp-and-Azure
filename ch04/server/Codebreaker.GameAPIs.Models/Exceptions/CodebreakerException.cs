using System.Diagnostics.CodeAnalysis;

using Codebreaker.GameAPIs.Extensions;

namespace Codebreaker.GameAPIs.Exceptions;

public class CodebreakerException : Exception
{

    public string Code { get; set; } = string.Empty;

	public CodebreakerException() { }
	public CodebreakerException(string message) : base(message) { }
	public CodebreakerException(string message, Exception inner) : base(message, inner) { }

	public static void ThrowIfNull([NotNull] Game? game)
	{
        if (game is null)
        {
            throw new CodebreakerException("Game not found") { Code = CodebreakerExceptionCodes.GameNotFound };
        }
    }

    public static void ThrowIfUnexpectedGameType(Game game, string gameType)
    {
        if (game.GameType != gameType)
        {
            throw new CodebreakerException("Game type not expected") { Code = CodebreakerExceptionCodes.UnexpectedGameType };
        }
    }

    public static void ThrowIfEnded(Game game)
    {
        if (game.HasEnded())
        {
            throw new CodebreakerException("Game is not active") { Code = CodebreakerExceptionCodes.GameNotActive };
        }
    }
}
