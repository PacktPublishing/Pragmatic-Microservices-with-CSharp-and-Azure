using Codebreaker.GameAPIs.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using System.Diagnostics.Metrics;

namespace Codebreaker.GameAPIs.Tests;

public class GamesMetricsTests
{
    private Guid _gameId = Guid.Parse("DBDF4DD9-3A02-4B2A-87F6-FFE4BA1DCE52");
    private DateTime _gameStartTime = new DateTime(2024, 1, 1, 12, 10, 5);
    private DateTime _gameMove1Time = new DateTime(2024, 1, 1, 12, 10, 15);
    private DateTime _gameMove2Time = new DateTime(2024, 1, 1, 12, 10, 30);

    [Fact]
    public void GameStarted_Should_IncrementActiveGamesCounter()
    {
        // arrange
        (IMeterFactory meterFactory, GamesMetrics metrics) = CreateMeterFactorySkeleton();
        MetricCollector<long> collector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.active_games");
        var game = GetGame();

        // act
        metrics.GameStarted(game);

        // assert
        var measurements = collector.GetMeasurementSnapshot();
        Assert.Single(measurements);
        Assert.Equal(1, measurements[0].Value);
    }

    [Fact]
    public void MoveSet_Should_Record_ThinkTime()
    {
        // arrange
        (IMeterFactory meterFactory, GamesMetrics metrics) = CreateMeterFactorySkeleton();
        MetricCollector<double> collector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.move_think_time");
        var game = GetGame();
        metrics.GameStarted(game);

        // act
        metrics.MoveSet(game.Id, _gameMove1Time, "Game6x4");

        // assert
        var measurements = collector.GetMeasurementSnapshot();
        Assert.Single(measurements);
        Assert.Equal(10, measurements[0].Value);
    }

    [Fact]
    public void GameEnded_Should_RecordValues()
    {
        // arrange
        (IMeterFactory meterFactory, GamesMetrics metrics) = CreateMeterFactorySkeleton();
        MetricCollector<double> gameDurationCollector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.game_duration");
        MetricCollector<long> activeGamesCollector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.active_games");
        MetricCollector<int> movesPerGameWinCollector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.game_moves-per-win");
        MetricCollector<long> gamesWonCollector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.games.won");
        MetricCollector<long> gamesLostCollector = new(meterFactory, GamesMetrics.MeterName, "codebreaker.games.lost");

        var game = GetGame();
        metrics.GameStarted(game);
        metrics.MoveSet(game.Id, _gameMove1Time, "Game6x4");
        metrics.MoveSet(game.Id, _gameMove2Time, "Game6x4");

        // act
        game.EndTime = _gameMove2Time;
        game.IsVictory = true;
        game.LastMoveNumber = 2;
        game.Duration = TimeSpan.FromSeconds(25);
        metrics.GameEnded(game);

        // assert
        var gameDurationMeasurements = gameDurationCollector.GetMeasurementSnapshot();
        Assert.Single(gameDurationMeasurements);
        Assert.Equal(25, gameDurationMeasurements[0].Value);

        var activeGamesMeasurements = activeGamesCollector.GetMeasurementSnapshot();
        Assert.Equal(2, activeGamesMeasurements.Count);
        Assert.Equal(-1, activeGamesMeasurements.Last().Value);

        var movesPerGameWinMeasurements = movesPerGameWinCollector.GetMeasurementSnapshot();
        Assert.Single(movesPerGameWinMeasurements);
        Assert.Equal(2, movesPerGameWinMeasurements[0].Value);

        var gamesWonMeasurements = gamesWonCollector.GetMeasurementSnapshot();
        Assert.Single(gamesWonMeasurements);
        Assert.Equal(1, gamesWonMeasurements[0].Value);

        var gamesLostMeasurements = gamesLostCollector.GetMeasurementSnapshot();
        Assert.Empty(gamesLostMeasurements);       
    }

    static void OnMeasurementRecorded<T>(
        Instrument instrument,
        T measurement,
        ReadOnlySpan<KeyValuePair<string, object?>> tags,
        object? state)
    {
        Console.WriteLine($"{instrument.Name} recorded measurement {measurement}");
    }

    private static IServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        services.AddMetrics();
        services.AddSingleton<GamesMetrics>();
        return services.BuildServiceProvider();
    }

    private static (IMeterFactory MeterFactory, GamesMetrics Metrics) CreateMeterFactorySkeleton()
    {
        var container = CreateServiceProvider();
        GamesMetrics metrics = container.GetRequiredService<GamesMetrics>();
        IMeterFactory meterFactory = container.GetRequiredService<IMeterFactory>();
        return (meterFactory, metrics);
    }

    public Game GetGame() => new Game(_gameId, "Game6x4", "Test", _gameStartTime, 4, 12)
    {
        Codes = ["Red", "Green", "Blue", "Yellow"],
        FieldValues = new Dictionary<string, IEnumerable<string>>()
            {
                { FieldCategories.Colors, ["Red", "Green", "Blue", "Yellow", "Purple", "Orange"] }
            }
    };
}
