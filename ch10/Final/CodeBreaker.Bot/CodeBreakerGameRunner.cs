using Codebreaker.GameAPIs.Client.Models;

namespace CodeBreaker.Bot;

public class CodeBreakerGameRunner(GamesClient gamesClient, ILogger<CodeBreakerGameRunner> logger)
{
    private const string PlayerName = "Bot";
    private Guid? _gameId;
    private int _moveNumber = 0;
    private readonly List<Move> _moves = [];
    private List<int>? _possibleValues;
    private Dictionary<int, string>? _colorNames;
    private readonly ILogger _logger = logger;
    private readonly GamesClient _gamesClient = gamesClient;

    // initialize a list of all the possible options using numbers for every color
    private List<int> InitializePossibleValues()
    {
        static List<int> Create8Colors(int shift)
        {
            List<int> pin = [];
            for (int i = 0; i < 6; i++)
            {
                int x = 1 << i + shift;
                pin.Add(x);
            }
            return pin;
        }

        static List<int> AddColorsToList(List<int> list1, List<int> list2)
        {
            List<int> result = new(capacity: 1300);
            for (int i = 0; i < list1.Count; i++)
            {
                for (int j = 0; j < list2.Count; j++)
                {
                    int x = list1[i] ^ list2[j];
                    result.Add(x);
                }
            }
            return result;
        }

        var digits1 = Create8Colors(0);
        var digits2 = Create8Colors(6);
        var list2 = AddColorsToList(digits1, digits2);
        var digits3 = Create8Colors(12);
        var list3 = AddColorsToList(list2, digits3);
        var digits4 = Create8Colors(18);
        var list4 = AddColorsToList(list3, digits4);
        list4.Sort();
        return list4;
    }

    public async Task StartGameAsync(CancellationToken cancellationToken = default)
    {
        static int NextKey(ref int key)
        {
            int next = key;
            key <<= 1;
            return next;
        }

        _possibleValues = InitializePossibleValues();
        _moves.Clear();
        _moveNumber = 0;

        (_gameId, _, _, IDictionary<string, string[]> fieldValues) = await _gamesClient.StartGameAsync(GameType.Game6x4, "Bot", cancellationToken);
        int key = 1;
        _colorNames = fieldValues["colors"]
            .ToDictionary(keySelector: c => NextKey(ref key), elementSelector: color => color);
    }

    /// <summary>
    /// Sends moves to the API server
    /// Moves are delayed by thinkSeconds before setting the next move
    /// Finishes when the game is over
    /// </summary>
    /// <param name="thinkSeconds">The seconds to simulate thinking before setting the next move</param>
    /// <returns>a task</returns>
    /// <exception cref="InvalidOperationException">throws if initialization was not done, or with invalid game state</exception>
    public async Task RunAsync(int thinkSeconds, CancellationToken cancellationToken = default)
    {
        if (_possibleValues is null) 
            throw new InvalidOperationException($"call {nameof(StartGameAsync)} before");
        Guid gameId = _gameId ?? 
            throw new InvalidOperationException($"call {nameof(StartGameAsync)} before");

        bool ended = false;
        do
        {
            _moveNumber++;
            (string[] guessPegs, int selection) = GetNextMoves();
            _logger.SendMove(string.Join(':', guessPegs), gameId.ToString());

            (string[] results, ended, bool isVictory) = await _gamesClient.SetMoveAsync(gameId, PlayerName, GameType.Game6x4, _moveNumber, guessPegs, cancellationToken);

            if (isVictory)
            {
                _logger.Matched(_moveNumber, gameId.ToString());
                break;
            }

            int blackHits = results.Count(c => c == "Black");
            int whiteHits = results.Count(c => c == "White");

            if (blackHits >= 4)
                throw new InvalidOperationException($"4 or more blacks but won was not set: {blackHits}");

            if (whiteHits > 4)
                throw new InvalidOperationException($"more than 4 whites is not possible: {whiteHits}");

            if (blackHits == 0 && whiteHits == 0)
            {
                _possibleValues = _possibleValues.HandleNoMatches(selection);
                _logger.ReducedPossibleValues(_possibleValues.Count, "none", gameId.ToString());
            }
            if (blackHits > 0)
            {
                _possibleValues = _possibleValues.HandleBlackMatches(blackHits, selection);
                _logger.ReducedPossibleValues(_possibleValues.Count, "Black", gameId.ToString());
            }
            if (whiteHits > 0)
            {
                _possibleValues = _possibleValues.HandleWhiteMatches(whiteHits + blackHits, selection);
                _logger.ReducedPossibleValues(_possibleValues.Count, "White", gameId.ToString());
            }

            await Task.Delay(TimeSpan.FromSeconds(thinkSeconds), cancellationToken);  // thinking delay
        } while (!ended);

        _logger.FinishedRun(_moveNumber, gameId.ToString());
    }

    /// <summary>
    /// Get the values for the next move
    /// </summary>
    /// <returns>A string and int representation for the next moves</returns>
    /// <exception cref="InvalidOperationException">Throws if there are no calculated possible values left to chose from</exception>
    private (string[] Colors, int Selection) GetNextMoves()
    {
        if (_possibleValues?.Count is null or 0) 
            throw new InvalidOperationException("invalid number of possible values - 0");

        int random = Random.Shared.Next(_possibleValues.Count);
        int value = _possibleValues[random];

        return (IntToColors(value), value);
    }

    private string[] IntToColors(int value) =>
    [
        _colorNames?[value & 0b111111] ?? string.Empty,
        _colorNames?[(value >> 6) & 0b111111] ?? string.Empty,
        _colorNames?[(value >> 12) & 0b111111] ?? string.Empty,
        _colorNames?[(value >> 18) & 0b111111] ?? string.Empty
    ];
}

public enum CodeColors
{
    Black = 1,
    White = 2,
    Red = 4,
    Green = 8,
    Blue = 16,
    Yellow = 32
}

public enum KeyColors
{
    Black,
    White
}

public record struct CodePeg(string Color)
{
    public override readonly string ToString() => Color;
}

public record struct KeyPeg(string Color)
{
    public override readonly string ToString() => Color;
}

public record struct Move(string GameId, int MoveNumber, IList<CodePeg> Codes, IList<KeyPeg> Keys);
