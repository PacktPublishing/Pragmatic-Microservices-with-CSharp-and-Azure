namespace Codebreaker.ViewModels.Models;

public partial class Game(
    Guid gameId,
    GameType gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves) : ObservableObject
{
    /// <summary>
    /// Gets the unique identifier of the game.
    /// </summary>
    public Guid GameId { get; private set; } = gameId;

    /// <summary>
    /// Gets the type of the game. <see cref="GameType"/>
    /// </summary>
    public GameType GameType { get; private set; } = gameType;

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string PlayerName { get; private set; } = playerName;

    /// <summary>
    /// Gets information if the player was authenticated while playing the game.
    /// </summary>
    public bool PlayerIsAuthenticated { get; set; } = false;

    /// <summary>
    /// Gets the start time of the game
    /// </summary>
    public DateTime StartTime { get; private set; } = startTime;

    /// <summary>
    /// Gets the end time of the game or null if it did not end yet. This value is set from a game guess anylzer after the game was ended.
    /// </summary>
    [ObservableProperty]
    private string? _endTime;

    /// <summary>
    /// Gets the duration of the game or null if it did not end yet
    /// </summary>
    [ObservableProperty]
    private TimeSpan? _duration;

    /// <summary>
    /// Gets the last move number. This number is set from an game move analyer after the move was set.
    /// </summary>  
    [ObservableProperty]
    private int _lastMoveNumber;

    /// <summary>
    /// Gets the number of codes the player needs to fill.
    /// </summary>  
    public int NumberCodes { get; private set; } = numberCodes;

    /// <summary>
    /// Gets the maximum number of moves the game ends when its not solved.
    /// </summary>
    public int MaxMoves { get; private set; } = maxMoves;

    /// <summary>
    /// Did the player win the game?
    /// </summary>  
    [ObservableProperty]
    private bool _isVictory = false;

    /// <summary>
    /// A list of possible field values the user has to chose from
    /// </summary>
    public required IDictionary<string, string[]> FieldValues { get; init; }

    /// <summary>
    /// A list of moves the player made
    /// </summary>
    public ICollection<Move> Moves { get; } = new ObservableCollection<Move>();

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}