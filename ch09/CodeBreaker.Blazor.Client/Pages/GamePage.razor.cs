using System.Timers;
using Codebreaker.GameAPIs.Client;
using Codebreaker.GameAPIs.Client.Models;
using CodeBreaker.Blazor.Client.Models;
using CodeBreaker.Blazor.Client.Resources;
using CodeBreaker.Blazor.Client.Components;
using CodeBreaker.Blazor.UI.Services.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace CodeBreaker.Blazor.Client.Pages;

public partial class GamePage : IDisposable
{
    private GameType _selectedGameType = GameType.Game6x4;

    //TODO: Get Data from API
    private readonly IEnumerable<KeyValuePair<string, GameType>> _gameTypes = [
        new ("8x5Game", GameType.Game8x5),
        new ("6x4MiniGame", GameType.Game6x4Mini),
        new ("6x4Game", GameType.Game6x4),
        new ("5x5x4Game", GameType.Game5x5x4),
    ];

    [Inject]
    private IGamesClient Client { get; init; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; init; } = default!;
    [Inject]
    private IJSRuntime JSRuntime { get; init; } = default!;
    [Inject]
    private IDialogService DialogService { get; init; } = default!;
    [Inject]
    private IStringLocalizer<Resource> Loc { get; init; } = default!;

    private readonly System.Timers.Timer _timer = new(TimeSpan.FromHours(1));
    private GameMode _gameStatus = GameMode.NotRunning;
    private string _name = string.Empty;
    private bool _loadingGame = false;
    private bool _cancelGame = false;
    private GameInfo? _game;

    protected override async Task OnInitializedAsync()
    {
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        //NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        _name = string.Empty;
        await base.OnInitializedAsync();
    }

    private async Task StartGameAsync()
    {
        try
        {
            _loadingGame = true;
            _gameStatus = GameMode.NotRunning;
            (Guid gameId, int numberCodes, int maxMoves, IDictionary<string, string[]> fieldValues) = await Client.StartGameAsync(_selectedGameType, _name);
            _game = new(gameId, _selectedGameType.ToString(), _name, DateTime.Now, numberCodes, maxMoves)
            {
                FieldValues = fieldValues.ToDictionary(x => x.Key, x => x.Value.AsEnumerable()),
                Codes = []
            };
            _gameStatus = GameMode.Started;
        }
        catch (Exception ex)
        {
            //TODO: Handle Exception
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _timer.Start();
            _loadingGame = false;
        }
    }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        await InvokeAsync(() =>
        {
            //TODO: Show dialog
            Console.WriteLine($"Time out called...Cancel game. Time {e.SignalTime}");
            _timer.Stop();
            _gameStatus = GameMode.Cancelled;
            StateHasChanged();
            DialogService.ShowDialog(new DialogContext(typeof(GameResultDialog), new Dictionary<string, object>
            {
                { nameof(GameResultDialog.GameMode), GameMode.Timeout },
                { nameof(GameResultDialog.Username), _name },
            }, string.Empty, [
                new DialogActionContext(Loc["GamePage_FinishGame_Ok"], () => NavigationManager.NavigateTo("")),
                new DialogActionContext(Loc["GamePage_FinishGame_Restart"], () => RestartGame()),
            ]));
        });
    }

    private void GameStatusChanged(GameMode gameMode)
    {
        _timer.Stop();
        _gameStatus = gameMode;
        if (_gameStatus is GameMode.Won or GameMode.Lost)
        {
            DialogService.ShowDialog(new DialogContext(typeof(GameResultDialog), new Dictionary<string, object>
            {
                { nameof(GameResultDialog.GameMode), _gameStatus },
                { nameof(GameResultDialog.Username), _name },
            }, string.Empty, [
                new DialogActionContext(Loc["GamePage_FinishGame_Ok"], () => NavigationManager.NavigateTo(string.Empty)),
                new DialogActionContext(Loc["GamePage_FinishGame_Restart"], () => RestartGame()),
            ]));
        }
        else
        {

            _timer.Start();
        }
    }

    private void CancelGame()
    {
        _timer.Stop();
        _gameStatus = GameMode.Cancelled;
        NavigationManager.NavigateTo(string.Empty);
    }

    private void RestartGame()
    {
        _game = null;
        _gameStatus = GameMode.NotRunning;
        StateHasChanged();
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (_gameStatus is GameMode.MoveSet)
        {
            var isConfirmed = await JSRuntime.InvokeAsync<bool>("confirm", Loc["GamePage_CancelGame_Info"]);

            if (!isConfirmed)
            {
                context.PreventNavigation();
            }
        }
    }

    // Cancelling game is not yet implemented by the API
    //private async ValueTask OnLocationChanging(LocationChangingContext context)
    //{
    //    if (_game is not null)
    //    {
    //        _cancelGame = true;
    //        await Client.CancelGameAsync(_game.Value.GameId);
    //        _cancelGame = false;
    //    }
    //}

    public void Dispose() => _timer?.Dispose();
}
