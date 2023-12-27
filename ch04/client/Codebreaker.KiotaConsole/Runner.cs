namespace Codebreaker.Client;

internal class RunnerOptions
{
    public required string GamesApiUrl { get; set; }
}

internal class Runner
{
    private readonly GamesAPIClient _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Runner(IOptions<RunnerOptions> options)
    {
        AnonymousAuthenticationProvider authenticationProvider = new();
        HttpClientRequestAdapter adapter = new(authenticationProvider)
        {
            BaseUrl = options.Value.GamesApiUrl ?? throw new InvalidOperationException("Could not read GamesApiUrl")
        };
        _client = new GamesAPIClient(adapter);
    }

    public async Task RunAsync()
    {
        bool ended = false;
        while (!ended)
        {
            var selection = Inputs.GetMainSelection();
            switch (selection)
            {
                case MainOptions.Play:
                    await PlayGameAsync();
                    break;
                case MainOptions.Exit:
                    ended = true;
                    break;
                case MainOptions.QueryGame:
                    await ShowGameAsync();
                    break;
                case MainOptions.QueryList:
                    await ShowGamesAsync();
                    break;
                case MainOptions.Delete:
                    await DeleteGameAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async Task DeleteGameAsync()
    {
        Guid gameId = Inputs.GetGameId();
        await _client.Games[gameId].DeleteAsync();
    }

    private async Task ShowGameAsync()
    {
        Guid gameId = Inputs.GetGameId();
        var game = await _client.Games[gameId].GetAsync(cancellationToken: _cancellationTokenSource.Token);
      
        if (game is null)
        {
            await Console.Out.WriteLineAsync($"Game {gameId} not found");
            return;
        }
        await Console.Out.WriteLineAsync($"Game found: {game}");
        await Console.Out.WriteLineAsync($"last move: {game.LastMoveNumber}");

        if (game.Moves is null)
            return;

        foreach (var move in game.Moves)
        {
            await Console.Out.WriteLineAsync(
                $"{move.MoveNumber}. " +
                $"{string.Join(':', move.GuessPegs ?? Enumerable.Empty<string>())} " +
                $"{string.Join(':', move.KeyPegs ?? Enumerable.Empty<string>())}");
        }
        await Console.Out.WriteLineAsync();
    }

    private async Task ShowGamesAsync()
    {
        var games = await _client.Games.GetAsync(config =>
        {
            config.QueryParameters.Date = new Date(DateTime.Today);
        }, _cancellationTokenSource.Token) ?? throw new InvalidOperationException();

        foreach (var game in games)
        {
            await Console.Out.WriteLineAsync($"{game.Id}, {game.GameType}");
        }
        await Console.Out.WriteLineAsync();
    }

    private async Task PlayGameAsync()
    {
        GameType gameType = Inputs.GetGameType();
        string playerName = Inputs.GetPlayername();

        static string[] ToStringArray(object o)
        {
            List<string> values = [];
            if (o is JsonElement je)
            {
                foreach (var s in je.EnumerateArray())
                {
                    values.Add(s.GetString() ?? string.Empty);
                }
                return [.. values];
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        (Guid gameId, int numberCodes, int maxMoves, IDictionary<string, string[]>? fields) = await AnsiConsole.Status().StartAsync<(Guid, int, int, IDictionary<string, string[]>)>("Starting game...", async _ =>
        {
            CreateGameRequest request = new()
            {
                PlayerName = playerName,
                GameType = gameType
            };
            var response =
                await _client.Games.PostAsync(request, cancellationToken: _cancellationTokenSource.Token)
                    ?? throw new InvalidOperationException();

            IDictionary<string, string[]> fieldValues = response?.FieldValues?.AdditionalData
            .ToDictionary(
                entry => entry.Key,
                entry => ToStringArray(entry.Value)) ?? throw new InvalidOperationException();

            return (response.Id!.Value, response.NumberCodes!.Value, response.MaxMoves!.Value, fieldValues);
        });

        int moveNumber = 0;
        bool ended = false;
        bool isVictory = false;
        do
        {
            moveNumber++;
            string[] guesses = Inputs.GetFieldChoices(numberCodes, fields);
            await Console.Out.WriteAsync($"{moveNumber}. {string.Join(" ", guesses.Select(s => s.PadRight(8, ' ')))}");
            UpdateGameRequest updateGameRequest = new()
            {
                MoveNumber = moveNumber,
                GuessPegs = [.. guesses]
            };
            UpdateGameResponse? response =
                await _client.Games[gameId].PatchAsync(updateGameRequest, cancellationToken: _cancellationTokenSource.Token)
                    ?? throw new InvalidOperationException();

            await Console.Out.WriteLineAsync($" ** {string.Join(' ', response.Results ?? Enumerable.Empty<string>())}");
        } while (!ended);
        await Console.Out.WriteLineAsync($"Victory: {isVictory}");
        string wonOrLost = isVictory ? "won" : "lost";
        await Console.Out.WriteLineAsync($"You {wonOrLost} after {moveNumber} moves");
    }
}
