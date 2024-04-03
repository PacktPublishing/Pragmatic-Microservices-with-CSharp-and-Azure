using Codebreaker.GameAPIs.Models;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace LiveTestClient;

internal class LiveClient(IOptions<LiveClientOptions> options) : IAsyncDisposable
{
    private HubConnection? _hubConnection;

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    public async Task StartMonitorAsync(CancellationToken cancellationToken = default)
    {
        string liveUrl = options.Value.LiveUrl ?? throw new InvalidOperationException("LiveUrl not configured");
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(liveUrl)
            .Build();

        _hubConnection.On("GameCompleted", (GameSummary summary) =>
        {
            Console.WriteLine($"Game {summary.Id} completed by {summary.PlayerName} after {summary.NumberMoves}");
        });

        await _hubConnection.StartAsync(cancellationToken);
    }

    public async Task SubscribeToGame(string gameType, CancellationToken cancellationToken = default)
    {
        if (_hubConnection is null) throw new InvalidOperationException("Start connection first");

        await _hubConnection.InvokeAsync("SubscribeToGameCompletions", gameType, cancellationToken);
    }
}

public class LiveClientOptions
{
    public string? LiveUrl { get; set; }
}