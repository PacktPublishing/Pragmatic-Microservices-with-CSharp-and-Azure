using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Codebreaker.GameAPIs.Infrastructure;

public sealed class GamesMetrics : IDisposable
{
    public const string MeterName = "Codebreaker.Games";
    public const string Version = "1.0";
    private readonly Meter _meter;

    private readonly UpDownCounter<long> _activeGamesCounter;
    private readonly Histogram<double> _gameDuration;
    private readonly Histogram<double> _moveThinkTime;
    private readonly Histogram<int> _movesPerGameWin;
    private readonly Counter<long> _invalidMoveCounter;
    private readonly Counter<long> _gamesWonCounter;
    private readonly Counter<long> _gamesLostCounter;

    private readonly ConcurrentDictionary<Guid, DateTime> _moveTimes = new();

    public GamesMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create(MeterName, Version);

        _activeGamesCounter = _meter.CreateUpDownCounter<long>(
            "codebreaker.active_games",
            unit: "{games}",
            description: "Number of games that are currently active on the server.");

        _gameDuration = _meter.CreateHistogram<double>(
            "codebreaker.game_duration",
            unit: "s",
            description: "Duration of a game in seconds.");

        _moveThinkTime = _meter.CreateHistogram<double>(
            "codebreaker.move_think_time",
            unit: "s",
            description: "Think time of a move in seconds.");

        _movesPerGameWin = _meter.CreateHistogram<int>(
            "codebreaker.game_moves-per-win",
            unit: "{moves}",
            description: "The number of moves needed for a game win");

        _invalidMoveCounter = _meter.CreateCounter<long>(
            "codebreaker.invalid_moves",
            unit: "{moves}",
            description: "Number of invalid moves.");

        _gamesWonCounter = _meter.CreateCounter<long>(
            "codebreaker.games.won",
            unit: "{won}",
            description: "Number of games won.");

        _gamesLostCounter = _meter.CreateCounter<long>(
            "codebreaker.games.lost",
            unit: "{lost}",
            description: "Number of games lost.");
    }

    private static KeyValuePair<string, object?> CreateGameTypeTag(string gameType) => KeyValuePair.Create<string, object?>("GameType", gameType);
    private static KeyValuePair<string, object?> CreateGameIdTag(Guid id) => KeyValuePair.Create<string, object?>("GameId", id.ToString());

    public void GameStarted(Game game)
    {
        if (_moveThinkTime.Enabled)
        {
            _moveTimes.TryAdd(game.Id, game.StartTime);
        }

        if (_activeGamesCounter.Enabled)
        {
            _activeGamesCounter.Add(1, CreateGameTypeTag(game.GameType));
        }
    }

    public void MoveSet(Guid id, DateTime moveTime, string gameType)
    {
        if (_moveThinkTime.Enabled)
        {
            _moveTimes.AddOrUpdate(id, moveTime, (id1, prevTime) =>
            {
                _moveThinkTime.Record((moveTime - prevTime).TotalSeconds, [CreateGameIdTag(id1), CreateGameTypeTag(gameType)]);
                return moveTime;
            });
        }
    }

    public void InvalidMove()
    {
        if (_invalidMoveCounter.Enabled)
        {
            _invalidMoveCounter.Add(1);
        }
    }

    public void GameEnded(Game game)
    {
        if (!game.HasEnded())
        {
            return;
        }
        if (_gameDuration.Enabled && game.Duration is not null)
        {
            _gameDuration.Record(game.Duration.Value.TotalSeconds, CreateGameTypeTag(game.GameType)); // game.Duration is not null if Ended() is true
        }
        if (_activeGamesCounter.Enabled)
        {
            _activeGamesCounter.Add(-1, CreateGameTypeTag(game.GameType));
        }
        if (game.IsVictory && _movesPerGameWin.Enabled)
        {
            _movesPerGameWin.Record(game.LastMoveNumber, CreateGameTypeTag(game.GameType));
        }
        if (game.IsVictory && _gamesWonCounter.Enabled)
        {
            _gamesWonCounter.Add(1, CreateGameTypeTag(game.GameType));
        }
        if (!game.IsVictory && _gamesLostCounter.Enabled)
        {
            _gamesLostCounter.Add(1, CreateGameTypeTag(game.GameType));
        }

        _moveTimes.TryRemove(game.Id, out _);
    }

    public void Dispose() => _meter?.Dispose();
}