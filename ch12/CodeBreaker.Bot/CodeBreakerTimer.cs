using CodeBreaker.Bot.Api;
using CodeBreaker.Bot.Exceptions;

namespace CodeBreaker.Bot;

public class CodeBreakerTimer(CodeBreakerGameRunner runner, ILogger<CodeBreakerTimer> logger)
{
    private readonly CodeBreakerGameRunner _gameRunner = runner;

    private static readonly ConcurrentDictionary<Guid, CodeBreakerTimer> _bots = new();

    private PeriodicTimer? _timer;

    private int _loop = 0;
    private string _statusMessage = "running";

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Guid Start(int delaySecondsBetweenGames, int numberGames, int thinkSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(delaySecondsBetweenGames);
        ArgumentOutOfRangeException.ThrowIfLessThan(numberGames, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(thinkSeconds);

        logger.StartGameRunner();
        Guid id = Guid.NewGuid();
        _bots.TryAdd(id, this);

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(delaySecondsBetweenGames));
     
        Task _ = Task.Factory.StartNew(async () =>
        {
            try
            {
                do
                {
                    logger.WaitingForNextTick(_loop);

                    if (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token)) // simulate some waiting time
                    {
                        logger.TimerTickFired(_loop);
                        await _gameRunner.StartGameAsync(_cancellationTokenSource.Token);  // start the game
                        await _gameRunner.RunAsync(thinkSeconds, _cancellationTokenSource.Token); // play the game until finished
                        _loop++;
                    }

                } while (_loop < numberGames);
            }
            catch (HttpRequestException ex)
            {
                _statusMessage = ex.Message;
                logger.Error(ex, ex.Message);
            }

        }, TaskCreationOptions.LongRunning);

        return id;
    }

    public void Stop()
    {
        _statusMessage = "stopped";
        _timer?.Dispose();
    }

    public StatusResponse Status() => 
        new StatusResponse(_loop + 1, _statusMessage);

    public static void Stop(Guid id)
    {
        if (id == default)
            throw new ArgumentException("Invalid argument value {id}", nameof(id));

        if (_bots.TryGetValue(id, out CodeBreakerTimer? timer))
        {
            timer.Stop();
            _bots.TryRemove(id, out _);
        }

        throw new BotNotFoundException();
    }

    public static StatusResponse Status(Guid id)
    {
        if (id == default)
            throw new ArgumentException("Invalid argument value {id}", nameof(id));

        if (_bots.TryGetValue(id, out CodeBreakerTimer? timer))
            return timer?.Status() ?? throw new UnknownStatusException("id found, but unknown status");

        throw new BotNotFoundException();
    }
}
