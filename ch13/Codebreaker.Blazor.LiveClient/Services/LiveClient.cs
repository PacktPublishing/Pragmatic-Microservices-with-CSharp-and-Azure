using Codebreaker.GameAPIs.Models;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.ServiceDiscovery;

namespace Codebreaker.Blazor.LiveClient.Services;

public class LiveClient(ServiceEndpointResolver endpointResolver)
{
    private HubConnection? _hubConnection;
    private readonly List<GameSummary> _gameSummaries = [];
    public IReadOnlyList<GameSummary> GameSummaries => _gameSummaries;
    public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;

    public event Func<Task>? StateChanged;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection != null)
            return;

        string? url = Environment.GetEnvironmentVariable("services__live__https__0");
        if (url is null) throw new InvalidOperationException("SignalR service URL not found.");
        url += "/livesubscribe";

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                logging.AddConsole();
            })
            .Build();

        _hubConnection.Closed += async (error) =>
        {
            if (StateChanged is not null) await StateChanged.Invoke();
        };

        _hubConnection.Reconnected += connectionId =>
        {
            if (StateChanged is not null) return StateChanged.Invoke();
            return Task.CompletedTask;
        };

        _hubConnection.Reconnecting += error =>
        {
            if (StateChanged is not null) return StateChanged.Invoke();
            return Task.CompletedTask;
        };

        _hubConnection.On<string>("OnError", (errorMessage) =>
        {
            Console.Error.WriteLine($"SignalR error: {errorMessage}");
        });

        _hubConnection.On<GameSummary>("GameCompleted", (gameSummary) =>
        {
            _gameSummaries.Add(gameSummary);
            if (StateChanged is not null) _ = StateChanged.Invoke();
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection is null) 
            throw new InvalidOperationException("Hub connection is not initialized.");

        await _hubConnection.StartAsync(cancellationToken);

        if (StateChanged is not null) 
            await StateChanged.Invoke();

        await _hubConnection.InvokeAsync("SubscribeToGameCompletions", "Game6x4", cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection is null) 
            throw new InvalidOperationException("Hub connection is not initialized.");

        await _hubConnection.InvokeAsync("UnsubscribeFromGameCompletions", "Game6x4", cancellationToken);
        await _hubConnection.StopAsync(cancellationToken);
        if (StateChanged is not null) 
            await StateChanged.Invoke();
    }

    public void Clear()
    {
        _gameSummaries.Clear();
        if (StateChanged is not null) _ = StateChanged.Invoke();
    }
}
