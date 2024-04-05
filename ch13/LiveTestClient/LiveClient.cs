

namespace LiveTestClient;

internal class LiveClient(IOptions<LiveClientOptions> options, ILogger<LiveClient> logger) : IAsyncDisposable
{
    private HubConnection? _hubConnection;

    public async Task StartMonitorAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string liveUrl = options.Value.LiveUrl ?? throw new InvalidOperationException("LiveUrl not configured");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(liveUrl)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .AddMessagePackProtocol()
                .Build();

            _hubConnection.On("GameCompleted", (GameSummary summary) =>
            {
                string status = summary.IsVictory ? "won" : "lost";
                Console.WriteLine($"Game {summary.Id} {status} by {summary.PlayerName} after " +
                    $"{summary.Duration:g} with {summary.NumberMoves} moves");
            });

            await _hubConnection.StartAsync(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex.Message);
        }
    }

    public async Task SubscribeToGame(string gameType, CancellationToken cancellationToken = default)
    {
        if (_hubConnection is null) throw new InvalidOperationException("Start a connection first!");

        try
        {
            await _hubConnection.InvokeAsync("SubscribeToGameCompletions", gameType, cancellationToken);
        }
        catch (HubException ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}

public class LiveClientOptions
{
    public string? LiveUrl { get; set; }
}
