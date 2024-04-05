using System.Net.Mail;

namespace Codebreaker.GameAPIs.Tests;

public class GameExtensionsTests
{
    [Fact]
    public void ToBytesAndToGame_Should_ReturnCreatedGameValues()
    {
        Game game = GamesFactory.CreateGame("Game6x4", "test");
        byte[] data = GameExtensions.ToBytes(game);
        Game? actualGame = GameExtensions.ToGame(data);

        Assert.Equivalent(game, actualGame, strict: true);
    }

    [Fact]
    public void ToBytesAndToGame_Should_ReturnGameWithMoveValues()
    {
        Game game = GamesFactory.CreateGame("Game6x4", "test");
        game.ApplyMove(["Red", "Green", "Blue", "Yellow"], 1);

        byte[] data = GameExtensions.ToBytes(game);
        Game? actualGame = GameExtensions.ToGame(data);

        Assert.Equivalent(game, actualGame, strict: true);
    }

    [Fact]
    public void ToStringAndToGame_Should_ReturnGameWithMoveValues()
    {
        Game game = GamesFactory.CreateGame("Game6x4", "test");
        game.ApplyMove(["Red", "Green", "Blue", "Yellow"], 1);

        string json = GameExtensions.ToJson(game);
        Game? actualGame = GameExtensions.ToGame(json);

        Assert.Equivalent(game, actualGame, strict: true);
    }
}
