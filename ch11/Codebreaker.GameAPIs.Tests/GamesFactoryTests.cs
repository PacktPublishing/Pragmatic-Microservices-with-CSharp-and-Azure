namespace Codebreaker.GameAPIs.Tests;

public class GamesFactoryTests
{
    [Fact]
    public void Create6x4GameShouldCreate6x4Game()
    {       
        Game game = GamesFactory.CreateGame("Game6x4", "Test");

        Assert.Equal("Test", game.PlayerName);
        Assert.Equal(4, game.Codes.Length);
        int colors = game.FieldValues["colors"].Count();
        Assert.Equal(6, colors);
    }

    [Fact]
    public void Create8x5GameShouldCreate8x5Game()
    {
        Game game = GamesFactory.CreateGame("Game8x5", "Test");

        Assert.Equal("Test", game.PlayerName);
        Assert.Equal(5, game.Codes.Length);
        int colors = game.FieldValues["colors"].Count();
        Assert.Equal(8, colors);
    }

    [Fact]
    public void Create5x5x4GameShouldCreate5x5x4Game()
    {
        Game game = GamesFactory.CreateGame("Game5x5x4", "Test");

        Assert.Equal("Test", game.PlayerName);
        Assert.Equal(4, game.Codes.Length);
        int colors = game.FieldValues["colors"].Count();
        Assert.Equal(5, colors);
        int shapes = game.FieldValues["shapes"].Count();
        Assert.Equal(5, shapes);
    }

    [Fact]
    public void Apply6x4MoveShouldAddAMoveToTheGame()
    {
        Game game = GamesFactory.CreateGame("Game6x4", "Test");
        var values = game.FieldValues["colors"];
        string[] guesses = Enumerable.Repeat(values.First(), 4).ToArray();
        game.ApplyMove(guesses, 1);

        Assert.Single(game.Moves);
    }

    [Fact]
    public void Apply8x5MoveShouldAddAMoveToTheGame()
    {
        Game game = GamesFactory.CreateGame("Game8x5", "Test");
        var values = game.FieldValues["colors"];
        string[] guesses = Enumerable.Repeat(values.First(), 5).ToArray();
        game.ApplyMove(guesses, 1);

        Assert.Single(game.Moves);
    }

    [Fact]
    public void Apply5x5x4MoveShouldAddAMoveToTheGame()
    {
        Game game = GamesFactory.CreateGame("Game5x5x4", "Test");
        var colors = game.FieldValues["colors"];
        var shapes = game.FieldValues["shapes"];
        string color = colors.First();
        string shape = shapes.First();
        string[] guesses = Enumerable.Repeat($"{shape};{color}", 4).ToArray();
        game.ApplyMove(guesses, 1);

        Assert.Single(game.Moves);
    }
}
