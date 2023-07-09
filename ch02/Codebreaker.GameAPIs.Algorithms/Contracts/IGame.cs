namespace Codebreaker.GameAPIs.Contracts;

public interface IGame
{
    /// <summary>
    /// The unique identifier for the game
    /// </summary>
    Guid GameId { get; }

    /// <summary>
    /// The type of game - a string to allow for future expansion
    /// </summary>
    string GameType { get; }

    /// <summary>
    /// The number of available positions in a game that need to be set
    /// </summary>
    int NumberCodes { get; }

    /// <summary>
    /// The maximum number of moves allowed in a game
    /// </summary>
    int MaxMoves { get; }

    /// <summary>
    /// The start time of the game
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// The end time of the game
    /// </summary>
    DateTime? EndTime { get; set; }

    /// <summary>
    /// The duration of the game - this might be a different value to the difference between the start and end times
    /// In case the user didn't play and see the game, this time might not be calculated for the duration.
    /// However, currently the duration is calculated as the difference between the start and end times.
    /// </summary>
    TimeSpan? Duration { get; set; }

    /// <summary>
    /// Indicates whether the game has been won or not
    /// </summary>
    bool IsVictory { get; set; }

    /// <summary>
    /// The last move number that was played
    /// </summary>
    int LastMoveNumber { get; set; }

    /// <summary>
    /// The available field values the user can chose from to position the pegs
    /// </summary>
    IDictionary<string, IEnumerable<string>> FieldValues { get; }

    /// <summary>
    /// This is the code that the user needs to guess
    /// </summary>
    string[] Codes { get; }
}
