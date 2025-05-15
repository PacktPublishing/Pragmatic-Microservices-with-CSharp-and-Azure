using System.Diagnostics;

namespace CodeBreaker.Bot;

/// <summary>
/// Manages the execution of automated game sessions for the CodeBreaker game, allowing for the scheduling, monitoring,
/// and control of game loops.
/// </summary>
/// <remarks>This class provides functionality to start and stop automated game sessions, track their status, and
/// retrieve information about ongoing or completed sessions. Each instance of <see cref="CodeBreakerTimer"/> represents
/// a single game runner, and multiple runners can be managed concurrently using static methods.</remarks>
/// <param name="runner"></param>
/// <param name="logger"></param>
/// <param name="activitySource"></param>
public class CodeBreakerTimer(CodeBreakerGameRunner runner, ILogger<CodeBreakerTimer> logger, [FromKeyedServices("Codebreaker.Bot")] ActivitySource activitySource)
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
    private bool _stopped;

    /// <summary>
    /// Starts a new game runner with the specified configuration and returns a unique identifier for the runner.
    /// </summary>
    /// <remarks>The method initializes a periodic timer to control the delay between games and starts an
    /// asynchronous loop to manage the game execution. The returned <see cref="Guid"/> can be used to track or manage
    /// the game runner instance.</remarks>
    /// <param name="delaySecondsBetweenGames">The delay, in seconds, between consecutive games. Must be non-negative.</param>
    /// <param name="numberGames">The total number of games to run. Must be at least 1.</param>
    /// <param name="thinkSeconds">The maximum time, in seconds, allowed for thinking during each game. Must be non-negative.</param>
    /// <returns>A <see cref="Guid"/> that uniquely identifies the started game runner.</returns>
    public Guid Start(int delaySecondsBetweenGames, int numberGames, int thinkSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(delaySecondsBetweenGames);
        ArgumentOutOfRangeException.ThrowIfLessThan(numberGames, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(thinkSeconds);

        _logger.StartGameRunner();

        var id = Guid.CreateVersion7();
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
                else
                {
                    _logger.TimerTickFailed(_loop);
                    // Check if we should continue or break
                    if (_stopped || _cancellationTokenSource.IsCancellationRequested)
                        break;
                }

            } while (_loop < numberGames && !_stopped && !_cancellationTokenSource.IsCancellationRequested);
        }
        catch (OperationCanceledException ex)
        {
            _statusMessage = "canceled";
            _logger.Error(ex, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            _statusMessage = ex.Message;
            _logger.Error(ex, ex.Message);
        }
        finally
        {
            _timer?.Dispose();
            _cancellationTokenSource.Dispose();
            _bots.TryRemove(id, out _);
        }
    }

    /// <summary>
    /// Stops the current operation and cancels any ongoing tasks.
    /// </summary>
    /// <remarks>This method sets the internal state to indicate that the operation has been stopped  and
    /// triggers cancellation for any tasks associated with the operation.  Subsequent calls to this method have no
    /// effect if the operation is already stopped.</remarks>
    public void Stop()
    {
        if (_stopped) return;
        _statusMessage = "stopped";
        _stopped = true;
        _cancellationTokenSource.Cancel();
    }

    /// <summary>
    /// Retrieves the current status information, including the loop count and status message.
    /// </summary>
    /// <returns>A <see cref="StatusInfo"/> object containing the current loop count incremented by one  and the associated
    /// status message.</returns>
    public StatusInfo GetStatus()
    {
        return new StatusInfo(_loop + 1, _statusMessage);
    }

    /// <summary>
    /// Stops the timer associated with the specified bot identifier.
    /// </summary>
    /// <remarks>This method attempts to stop the timer for the bot identified by <paramref name="id"/>. If
    /// the bot is not found, a <see cref="BotNotFoundException"/> is thrown.</remarks>
    /// <param name="id">The unique identifier of the bot whose timer is to be stopped. Must not be <see cref="Guid.Empty"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is <see cref="Guid.Empty"/>.</exception>
    /// <exception cref="BotNotFoundException">Thrown if no bot with the specified <paramref name="id"/> is found.</exception>
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

    /// <summary>
    /// Retrieves the status information for a bot identified by the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the bot whose status is to be retrieved. Must not be <see cref="Guid.Empty"/>.</param>
    /// <returns>A <see cref="StatusInfo"/> object containing the current status of the bot.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is <see cref="Guid.Empty"/>.</exception>
    /// <exception cref="UnknownStatusException">Thrown if the bot is found but its status cannot be determined.</exception>
    /// <exception cref="BotNotFoundException">Thrown if no bot with the specified <paramref name="id"/> is found.</exception>
    public static StatusInfo GetStatus(Guid id)
    {
        if (id == default)
            throw new ArgumentException("Invalid argument value {id}", nameof(id));

        if (_bots.TryGetValue(id, out CodeBreakerTimer? timer))
            return timer?.GetStatus() ?? throw new UnknownStatusException("id found, but unknown status");

        throw new BotNotFoundException();
    }

    /// <summary>
    /// Retrieves the status information for all available bots.
    /// </summary>
    /// <remarks>This method aggregates the status information from all registered bots.  Each bot's status is
    /// represented as a <see cref="StatusInfo"/> object.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="StatusInfo"/> objects,  where each object represents the current
    /// status of a bot.  The collection will be empty if no bots are available.</returns>
    public static IEnumerable<StatusInfo> GetAllStatuses()
    {
        return [.. _bots.Select(b => b.Value.GetStatus())];
    }
}
