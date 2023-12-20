using Codebreaker.Client;
using Codebreaker.ViewModels.Components;
using Codebreaker.ViewModels.Contracts.Services;
using Microsoft.Extensions.Options;

namespace Codebreaker.ViewModels;

public enum GameMode
{
    NotRunning,
    Started,
    MoveSet,
    Lost,
    Won
}

public enum GameMoveValue
{
    Started,
    Completed
}

public partial class GamePageViewModel : ObservableObject
{
    private readonly IGamesClient _client;
    private int _moveNumber = 0;

    private readonly IDialogService _dialogService;
    private readonly IInfoBarService _infoBarService;
    private readonly AuthenticationService _authenticationService;

    public GamePageViewModel(
        IGamesClient client,
        IDialogService dialogService,
        IInfoBarService infoBarService,
        AuthenticationService authenticationService
    )
    {
        _client = client;
        _dialogService = dialogService;
        _infoBarService = infoBarService;
        _authenticationService = authenticationService;

        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(GameStatus))
                WeakReferenceMessenger.Default.Send(new GameStateChangedMessage(GameStatus));
        };
    }

    /// <summary>
    /// Information on the game - messages, errors, etc. See <see cref="InfoBarMessageService"/>.
    /// </summary>
    public InfoBarService InfoBarMessageService { get; } = new();

    private Game? _game;
    /// <summary>
    /// <see cref="Models.Game"/> instance."/>
    /// </summary>
    public Game? Game
    {
        get => _game;
        set
        {
            OnPropertyChanging(nameof(Game));
            OnPropertyChanging(nameof(Fields));
            _game = value;

            Fields.Clear();

            for (int i = 0; i < _game?.NumberCodes; i++)
            {
                SelectedFieldViewModel field = new();
                field.PropertyChanged += (sender, e) => SetMoveCommand.NotifyCanExecuteChanged();
                Fields.Add(field);
            }

            OnPropertyChanged(nameof(Game));
            OnPropertyChanged(nameof(Fields));
        }
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [NotifyPropertyChangedFor(nameof(IsNameEnterable))]
    [ObservableProperty]
    private bool _isNamePredefined = false;

    public ObservableCollection<SelectedFieldViewModel> Fields { get; } = [];

    public ObservableCollection<SelectionAndKeyPegs> GameMoves { get; } = [];

    /// <summary>
    /// Status of the game. See <see cref="GameMode"/>.
    /// </summary>
    [ObservableProperty]
    private GameMode _gameStatus = GameMode.NotRunning;

    /// <summary>
    /// An API call to the games-API is in-progress. Use to display progress indicators.
    /// </summary>
    [NotifyPropertyChangedFor(nameof(IsNameEnterable))]
    [ObservableProperty]
    private bool _inProgress = false;

    [ObservableProperty]
    private bool _isCancelling = false;

    /// <summary>
    /// The player name can be entered.
    /// </summary>
    public bool IsNameEnterable => !InProgress && !IsNamePredefined;

    /// <summary>
    /// Starts a new game using <see cref="IGamesClient"/>.
    /// Updates the <see cref="GameStatus"/> property.
    /// Initializes <see cref="Game"/>).
    /// Increments the move number.
    /// Shows <see cref="IDialogService"/> messages or <see cref="InfoBarMessageService"/> messages with errors.
    /// </summary>
    /// <returns>A task</returns>
    [RelayCommand(AllowConcurrentExecutions = false, FlowExceptionsToTaskScheduler = true)]
    private async Task StartGameAsync()
    {
        try
        {
            InitializeValues();
            var result = await _authenticationService.LoginAsync();

            InProgress = true;
            (Guid gameId, int numberCodes, int maxMoves, IDictionary<string, string[]> fieldValues) = await _client.StartGameAsync(GameType.Game6x4, Name);

            GameStatus = GameMode.Started;

            Game = new Game(gameId, GameType.Game6x4, Name, DateTime.Now, numberCodes, maxMoves)
            {
                FieldValues = fieldValues
            };
            _moveNumber++;
        }
        catch (Exception ex)
        {
            _infoBarService.New
                .IsErrorMessage()
                .WithMessage(ex.Message)
                .WithAction((message) =>
                {
                    GameStatus = GameMode.NotRunning;
                    message.Close();
                })
                .Show();
        }
        finally
        {
            InProgress = false;
        }
    }

    // TODO: end of the game is not yet implemented (in the client library)
//    [RelayCommand(AllowConcurrentExecutions = false, FlowExceptionsToTaskScheduler = true)]
//    private async Task CancelGameAsync()
//    {
//        if (Game is null)
//            throw new InvalidOperationException("No game running");

//        IsCancelling = true;

//        try
//        {
////            await _client.CancelGameAsync(Game!.Value.GameId);
//            GameStatus = GameMode.NotRunning;
//        }
//        catch (Exception ex)
//        {
//            InfoBarMessageService.ShowError(ex.Message);

//            if (_enableDialogs)
//                await _dialogService.ShowMessageAsync(ex.Message);
//        }
//        finally
//        {
//            IsCancelling = false;
//        }
//    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A task</returns>
    /// <exception cref="InvalidOperationException"></exception>
    [RelayCommand(CanExecute = nameof(CanSetMove), AllowConcurrentExecutions = false, FlowExceptionsToTaskScheduler = true)]
    private async Task SetMoveAsync()
    {
        try
        {
            InProgress = true;
            WeakReferenceMessenger.Default.Send(new GameMoveMessage(GameMoveValue.Started));

            if (_game is null)
                throw new InvalidOperationException("no game running");

            if (Fields.Count != _game.NumberCodes || Fields.Any(x => !x.IsSet))
                throw new InvalidOperationException("all colors need to be selected before invoking this method");

            string[] guessPegs = Fields.Select(x => x.Value!).ToArray();

            (string[] keyPegs, bool ended, bool isVictory) = await _client.SetMoveAsync(_game.GameId,  Name, GameType.Game6x4,  _moveNumber, guessPegs);

            SelectionAndKeyPegs selectionAndKeyPegs = new(guessPegs, keyPegs, _moveNumber++);
            GameMoves.Add(selectionAndKeyPegs);
            GameStatus = GameMode.MoveSet;

            WeakReferenceMessenger.Default.Send(new GameMoveMessage(GameMoveValue.Completed, selectionAndKeyPegs));

            if (isVictory)
            {
                GameStatus = GameMode.Won;
                InfoBarMessageService.New.IsSuccessMessage().WithMessage("Congratulations - you won!").Show();
            }
            else if (ended)
            {
                GameStatus = GameMode.Lost;
                InfoBarMessageService.New.WithMessage("Sorry, you didn't find the matching colors!").Show();
            }
        }
        catch (Exception ex)
        {
            InfoBarMessageService.New.WithMessage(ex.Message).IsErrorMessage().Show();
        }
        finally
        {
            ClearSelectedColor();
            InProgress = false;
        }
    }

    private bool CanSetMove =>
        Fields.All(s => s is not null && s.IsSet);

    private void ClearSelectedColor()
    {
        for (int i = 0; i < Fields.Count; i++)
            Fields[i].Reset();

        SetMoveCommand.NotifyCanExecuteChanged();
    }

    private void InitializeValues()
    {
        ClearSelectedColor();
        GameMoves.Clear();
        GameStatus = GameMode.NotRunning;
        InfoBarMessageService.Clear();
        _moveNumber = 0;
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="GuessPegs">String representation of guesses</param>
/// <param name="KeyPegs">String representation of results</param>
/// <param name="MoveNumber">The move number</param>
public record SelectionAndKeyPegs(string[] GuessPegs, string[] KeyPegs, int MoveNumber);

public record class GameStateChangedMessage(GameMode GameMode);

/// <summary>
/// Messages sent when the games starts, or a move is set.
/// </summary>
/// <param name="GameMoveValue"><see cref="GameMoveValue"/></param>
/// <param name="SelectionAndKeyPegs"><see cref="SelectionAndKeyPegs"/></param>
public record class GameMoveMessage(GameMoveValue GameMoveValue, SelectionAndKeyPegs? SelectionAndKeyPegs = null);
