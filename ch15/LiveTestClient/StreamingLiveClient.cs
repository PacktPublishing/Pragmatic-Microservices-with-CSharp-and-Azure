namespace LiveTestClient;

internal class StreamingLiveClient(IOptions<LiveClientOptions> options, ILogger<StreamingLiveClient> logger)
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

            await _hubConnection.StartAsync(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error: {Error}", ex.Message);
            throw;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation canceled");
        }
    }

    public async Task SubscribeToGame(string gameType, CancellationToken cancellationToken = default)
    {
        if (_hubConnection is null) throw new InvalidOperationException("Start a connection first!");

        try
        {
            if (_hubConnection.State != HubConnectionState.Connected)
            {
                logger.LogError("SignalR connection not active");
                return;
            }

            await foreach (GameSummary summary in _hubConnection.StreamAsync<GameSummary>("SubscribeToGameCompletions", gameType, cancellationToken))
            {
                string status = summary.IsVictory ? "won" : "lost";
                Console.WriteLine($"Game {summary.Id} {status} by {summary.PlayerName} after " +
                    $"{summary.Duration:g} with {summary.NumberMoves} moves");
            }
        }
        catch (HubException ex)
        {
            logger.LogError(ex, "Error: {Error}", ex.Message);
            throw;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation canceled");
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
