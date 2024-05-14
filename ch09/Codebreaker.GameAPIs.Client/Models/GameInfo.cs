namespace Codebreaker.GameAPIs.Client.Models;

/// <summary>
/// Complete information about a single game - including the soltuion and the moves the player made.
/// </summary>
/// <param name="gameId">The unique identifier of the game</param>
/// <param name="gameType"></param>
/// <param name="playerName"></param>
/// <param name="startTime"></param>
/// <param name="numberCodes"></param>
/// <param name="maxMoves"></param>
public class GameInfo(
    Guid gameId,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves)
{
    /// <summary>
    /// Gets the unique identifier of the game.
    /// </summary>
    public Guid GameId { get; private set; } = gameId;

    /// <summary>
    /// Gets the type of the game. <see cref="GameType"/>
    /// </summary>
    public string GameType { get; private set; } = gameType;

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
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets the duration of the game or null if it did not end yet
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets the last move number. This number is set from an game move analyer after the move was set.
    /// </summary>  
    public int LastMoveNumber { get; set; } = 0;

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
    public bool IsVictory { get; set; } = false;

    /// <summary>
    /// A list of possible field values the user has to chose from
    /// </summary>
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }

    /// <summary>
    /// This is the solution of the game with string representations of the codes
    /// </summary>
    public required string[] Codes { get; init; }

    /// <summary>
    /// A list of moves the player made
    /// </summary>
    public ICollection<MoveInfo> Moves { get; init; } = new List<MoveInfo>();

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}
