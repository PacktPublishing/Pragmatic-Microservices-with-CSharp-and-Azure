namespace Codebreaker.GameAPIs.Contracts;

/// <summary>
/// Implement this interface with a **game** class to use it from game guess analyzers.
/// </summary>
public interface IGame
{
    /// <summary>
    /// The unique identifier for the game
    /// </summary>
    Guid Id { get; }

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
    /// The end time of the game. 
    /// This is set by the game analyzer.
    /// </summary>
    DateTime? EndTime { get; set; }

    /// <summary>
    /// The duration of the game - this might be a different value to the difference between the start and end times
    /// In case the user didn't play and see the game, this time might not be calculated for the duration.
    /// Currently the duration is calculated as the difference between the start and end times. This might change in the future.
    /// </summary>
    TimeSpan? Duration { get; set; }

    /// <summary>
    /// Indicates whether the game has been won or not. This is set by the game analyzer.
    /// </summary>
    bool IsVictory { get; set; }

    /// <summary>
    /// The last move number that was played
    /// </summary>
    int LastMoveNumber { get; set; }

    /// <summary>
    /// The string representation of available field values the user can chose from to position the pegs.
    /// Multiple categories for the field values can be defined, such as *colors* and *shapes*.
    /// </summary>
    IDictionary<string, IEnumerable<string>> FieldValues { get; }

    /// <summary>
    /// This is the string representation of the code that the user needs to guess
    /// </summary>
    string[] Codes { get; }
}
