using Codebreaker.GameAPIs.Exceptions;

namespace Codebreaker.GameAPIs.Factories;

public static class GamesFactory
{
    private static readonly string[] s_colors6 = { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange };
    private static readonly string[] s_colors8 = { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown };
    private static readonly string[] s_colors5 = { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple };
    private static readonly string[] s_shapes5 = { Shapes.Circle, Shapes.Square, Shapes.Triangle, Shapes.Star, Shapes.Rectangle };

    public static Game CreateGame(string gameType, string playerName, bool isAuthenticated)
    {
        SimpleGame Create6x4SimpleGame() =>
            new (Guid.NewGuid(), gameType, playerName, isAuthenticated, DateTime.Now, 4, 12)
            {
                Fields = s_colors6.Select(c => new ColorField(c)).ToArray(),
                Codes = Random.Shared.GetItems(s_colors6, 4).Select(c => new ColorField(c)).ToArray()
            };

        ColorGame Create6x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, isAuthenticated, DateTime.Now, 4, 12)
            {
                Fields = s_colors6.Select(c => new ColorField(c)).ToArray(),
                Codes = Random.Shared.GetItems(s_colors6, 4).Select(c => new ColorField(c)).ToArray()
            };

        ColorGame Create8x5Game() =>
            new(Guid.NewGuid(), gameType, playerName, isAuthenticated, DateTime.Now, 5, 12)
            {
                Fields = s_colors8.Select(c => new ColorField(c)).ToArray(),
                Codes = Random.Shared.GetItems(s_colors8, 5).Select(c => new ColorField(c)).ToArray()
            };

        ShapeGame Create5x5x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, isAuthenticated, DateTime.Now, 4, 14)
            {
                Fields = Enumerable.Range(0, 5).Select(i => new ShapeAndColorField(s_shapes5[i], s_colors5[i])).ToArray(),
                Codes = Random.Shared.GetItems(s_shapes5, 4)
                    .Zip(Random.Shared.GetItems(s_colors5, 4), (s, c) => new ShapeAndColorField(s, c)).ToArray()    
            };

        return gameType switch
        {
            GameTypes.Game6x4Mini => Create6x4SimpleGame(),
            GameTypes.Game6x4 => Create6x4Game(),
            GameTypes.Game8x5 => Create8x5Game(),
            GameTypes.Game5x5x4 => Create5x5x4Game(),
            _ => throw new InvalidGameException("Invalid game type") { HResult = 4000 }
        };
    }
}
