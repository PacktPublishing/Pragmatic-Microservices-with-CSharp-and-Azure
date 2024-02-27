using Codebreaker.GameAPIs.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Codebreaker.GameAPIs.Tests;

public class GamesServiceTests
{
    private readonly Mock<IGamesRepository> _gamesRepositoryMock = new();
    private readonly Guid _endedGameId = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E6");
    private readonly Game _endedGame;
    private readonly Guid _running6x4GameId = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E7");
    private readonly Game _running6x4Game;
    private readonly Guid _notFoundGameId = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E8");
    private readonly Guid _running6x4MoveId1 = Guid.Parse("4786C27B-3F9A-4C47-9947-F983CF7053E9");
    private readonly string[] _guessesMove1 = ["Red", "Green", "Blue", "Yellow"];
    private readonly Mock<IDistributedCache> _distributedCacheMock = new();

    public GamesServiceTests()
    {
        _endedGame = new(_endedGameId, "Game6x4", "Test", DateTime.Now, 4, 12)
        {
            Codes = ["Red", "Green", "Blue", "Yellow"],
            FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                { FieldCategories.Colors, ["Red", "Green", "Blue", "Yellow", "Purple", "Orange"] }
            },
            EndTime = DateTime.Now.AddMinutes(3)
        };

        _running6x4Game = new(_running6x4GameId, "Game6x4", "Test", DateTime.Now, 4, 12)
        {
            Codes = ["Red", "Green", "Blue", "Yellow"],
            FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                { FieldCategories.Colors, ["Red", "Green", "Blue", "Yellow", "Purple", "Orange"] }
            }
        };

        _gamesRepositoryMock.Setup(repo => repo.GetGameAsync(_endedGameId, CancellationToken.None)).ReturnsAsync(_endedGame);
        _gamesRepositoryMock.Setup(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None)).ReturnsAsync(_running6x4Game);
        _gamesRepositoryMock.Setup(repo => repo.AddMoveAsync(_running6x4Game, It.IsAny<Move>(), CancellationToken.None));
    }

    //[Fact]
    //public async Task StartGame_Should_()
    //{
    //    GamesService gamesService = GetGamesService();
    //    await Assert.ThrowsAsync<CodebreakerException>(async () =>
    //    {
    //        await gamesService.StartGameAsync("Game6x4", "Test", CancellationToken.None);
    //    });
    //}

    [Fact]
    public async Task SetMoveAsync_Should_ThrowWithEndedGame()
    {
        GamesService gamesService = GetGamesService();
        await Assert.ThrowsAsync<CodebreakerException>(async () =>
        {
            await gamesService.SetMoveAsync(_endedGameId, "Game6x4", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);
        });

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_endedGameId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SetMoveAsync_Should_ThrowWithUnexpectedGameType()
    {
        GamesService gamesService = GetGamesService();
        await Assert.ThrowsAsync<CodebreakerException>(async () =>
        {
            await gamesService.SetMoveAsync(_running6x4GameId, "Game8x5", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);
        });

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SetMoveAsync_Should_ThrowWithNotFoundGame()
    {
        GamesService gamesService = GetGamesService();
        await Assert.ThrowsAsync<CodebreakerException>(async () =>
        {
            await gamesService.SetMoveAsync(_notFoundGameId, "Game6x4", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);
        });

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_notFoundGameId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetGameAsync_Should_ReturnAGame()
    {
        // Arrange
        GamesService gamesService = GetGamesService();

        // Act
        Game? game = await gamesService.GetGameAsync(_running6x4GameId, CancellationToken.None);

        // Assert
        Assert.Equal(_running6x4Game, game);
        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None), Times.Once);

    }

    [Fact]
    public async Task SetMoveAsync_Should_UpdateGameAndAddMove()
    {
        // Arrange
        GamesService gamesService = GetGamesService();

        // Act
        var result = await gamesService.SetMoveAsync(_running6x4GameId, "Game6x4", ["Red", "Green", "Blue", "Yellow"], 1, CancellationToken.None);

        // Assert
        Assert.Equal(_running6x4Game, result.Game);
        Assert.Single(result.Game.Moves);

        _gamesRepositoryMock.Verify(repo => repo.GetGameAsync(_running6x4GameId, CancellationToken.None), Times.Once);
        _gamesRepositoryMock.Verify(repo => repo.AddMoveAsync(_running6x4Game, It.IsAny<Move>(), CancellationToken.None), Times.Once);
    }

    private GamesService GetGamesService()
    {
        IMeterFactory meterFactory = new TestMeterFactory();
        GamesMetrics metrics = new(meterFactory);
        return new GamesService(_gamesRepositoryMock.Object, _distributedCacheMock.Object, NullLogger<GamesService>.Instance, metrics, new ActivitySource("TestSource"));
    }
}
