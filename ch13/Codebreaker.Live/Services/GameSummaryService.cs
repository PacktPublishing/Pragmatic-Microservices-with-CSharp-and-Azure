using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace Codebreaker.Live.Services;

public class GameSummaryService : IGameSummaryService
{
    private readonly FrozenDictionary<string, ConcurrentDictionary<DateTime, GameSummary>> gamesCollection = new Dictionary<string, ConcurrentDictionary<DateTime, GameSummary>>()
    {
        [GameTypes.Game6x4] = new(),
        [GameTypes.Game8x5] = new(),
        [GameTypes.Game5x5x4] = new(),
        [GameTypes.Game6x4Mini] = new()
    }.ToFrozenDictionary();

    public Task AddGameAsync(GameSummary gameSummary)
    {
        gamesCollection[gameSummary.GameType].AddOrUpdate(gameSummary.StartTime, gameSummary, (time, game) => game);
        return Task.CompletedTask;
    }

    //public async IAsyncEnumerable<GameSummary> GetGamesAsync(string gameType, [EnumeratorCancellation] CancellationToken token = default)
    //{
    //    // TODO: test for multi-threaded access
    //    foreach (GameSummary game in gamesCollection[gameType].Values)
    //    {
    //        yield return game;
    //        await Task.Delay(0);
    //    }
    //}

    public Task RemoveGamesAsync(DateTime before)
    {
        foreach (var collection in gamesCollection)
        {
            IEnumerable<DateTime> keys = collection.Value.Keys.Where(time => time < before);
            foreach (DateTime key in keys)
            {
                collection.Value.TryRemove(key, out _);
            }
        }
        return Task.CompletedTask;
    }
}
