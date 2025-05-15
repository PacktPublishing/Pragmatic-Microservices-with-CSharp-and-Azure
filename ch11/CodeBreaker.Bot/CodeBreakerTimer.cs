using System.Diagnostics;

namespace CodeBreaker.Bot;

public class CodeBreakerTimer(CodeBreakerGameRunner runner, ILogger<CodeBreakerTimer> logger, [FromKeyedServices("Codebreaker.Bot")] ActivitySource activitySource) : IDisposable
{
    private const string GameTypeTagName = "codebreaker.gameType";
    private const string GameSessionIdTagName = "codebreaker.gameSessionId";
    private const string GameLoopNumber = "codebreaker.gameLoopNumber";

    private readonly CodeBreakerGameRunner _gameRunner = runner;

    private readonly ILogger _logger = logger;

    private static readonly ConcurrentDictionary<Guid, CodeBreakerTimer> _bots = new();

    private PeriodicTimer? _timer;

    private int _loop = 0;
    private string _statusMessage = "running";

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private bool _disposed;

    public Guid Start(int delaySecondsBetweenGames, int numberGames, int thinkSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(delaySecondsBetweenGames);
        ArgumentOutOfRangeException.ThrowIfLessThan(numberGames, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(thinkSeconds);

        _logger.StartGameRunner();

        var id = Guid.NewGuid();
        _bots.TryAdd(id, this);

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(delaySecondsBetweenGames));
    
        Task _ = RunBotLoopAsync(id, numberGames, thinkSeconds); // fire-and-forget async

        return id;
    }

    private async Task RunBotLoopAsync(Guid id, int numberGames, int thinkSeconds)
    {
        if (_timer == null)
            throw new InvalidOperationException("Timer not initialized, invoke the Start method before!");

        try
        {
            do
            {
                using var activity = activitySource.StartActivity("BotPlayGame", ActivityKind.Client);
                activity?
                    .AddTag(GameTypeTagName, "Game6x4")
                    .AddTag(GameSessionIdTagName, id)
                    .AddTag(GameLoopNumber, _loop.ToString())
                    .Start();

                _logger.WaitingForNextTick(_loop);

                if (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token)) // simulate some waiting time
                {
                    _logger.TimerTickFired(_loop);
                    await _gameRunner.StartGameAsync(_cancellationTokenSource.Token);  // start the game
                    await _gameRunner.RunAsync(thinkSeconds, _cancellationTokenSource.Token); // play the game until finished
                    _loop++;
                }

            } while (_loop < numberGames);
        }
        catch (HttpRequestException ex)
        {
            _statusMessage = ex.Message;
            _logger.Error(ex, ex.Message);
        }
        finally
        {
            Dispose();
        }
    }

    public void Stop()
    {
        if (_disposed) return;
        _statusMessage = "stopped";
        _timer?.Dispose();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _timer?.Dispose();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _disposed = true;
    }

    public StatusInfo GetStatus()
    {
        return new StatusInfo(_loop + 1, _statusMessage);
    }

    public static void Stop(Guid id)
    {
        if (id == default)
            throw new ArgumentException("Invalid argument value {id}", nameof(id));

        if (_bots.TryRemove(id, out CodeBreakerTimer? timer))
        {
            timer.Stop();
            return;
        }

        throw new BotNotFoundException();
    }

    public static StatusInfo GetStatus(Guid id)
    {
        if (id == default)
            throw new ArgumentException("Invalid argument value {id}", nameof(id));

        if (_bots.TryGetValue(id, out CodeBreakerTimer? timer))
            return timer?.GetStatus() ?? throw new UnknownStatusException("id found, but unknown status");

        throw new BotNotFoundException();
    }

    public static IEnumerable<StatusInfo> GetAllStatuses()
    {
        return [.. _bots.Select(b => b.Value.GetStatus())];
    }
}
