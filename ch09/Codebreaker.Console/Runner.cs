namespace Codebreaker.Client;
internal class Runner(GamesClient client)
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

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
        await client.DeleteGameAsync(gameId);
    }

    private async Task ShowGameAsync()
    {
        Guid gameId = Inputs.GetGameId();
        var game = await client.GetGameAsync(gameId);
        if (game is null)
        {
            await Console.Out.WriteLineAsync($"Game {gameId} not found");
            return;
        }
        await Console.Out.WriteLineAsync($"Game found: {game}");
        await Console.Out.WriteLineAsync($"last move: {game.LastMoveNumber}");
        foreach (var move in game.Moves)
        {
            await Console.Out.WriteLineAsync(
                $"{move.MoveNumber}. " +
                $"{string.Join(':', move.GuessPegs)} " +
                $"{string.Join(':', move.KeyPegs)}");
        }
        await Console.Out.WriteLineAsync();
    }

    private async Task ShowGamesAsync()
    {
        GamesQuery query = new()
        {
            Date = DateOnly.FromDateTime(DateTime.Today)
        };
        var games = await client.GetGamesAsync(query);
        foreach (var game in games)
        {
            await Console.Out.WriteLineAsync(game.ToString());
        }
        await Console.Out.WriteLineAsync();
    }

    private async Task PlayGameAsync()
    {
        GameType gameType = Inputs.GetGameType();
        string playerName = Inputs.GetPlayername();
        (Guid gameId, int numberCodes, int maxMoves, IDictionary<string, string[]>? fields) = await AnsiConsole.Status().StartAsync("Starting game...", async _ =>
        {
            (Guid gameId, int numberCodes, int maxMoves, var fields) = await client.StartGameAsync(gameType, playerName, _cancellationTokenSource.Token);
            return (gameId, numberCodes, maxMoves, fields);
        });

        int moveNumber = 0;
        bool ended = false;
        bool isVictory = false;
        do
        {
            moveNumber++;
            string[] guesses = Inputs.GetFieldChoices(numberCodes, fields);
            await Console.Out.WriteAsync($"{moveNumber}. {string.Join(" ", guesses.Select(s => s.PadRight(8, ' ')))}");
            (string[] results, ended, isVictory) = await client.SetMoveAsync(gameId, playerName, gameType, moveNumber, guesses);
            await Console.Out.WriteLineAsync($" ** {string.Join(' ', results)}");
        } while (!ended);
        await Console.Out.WriteLineAsync($"Victory: {isVictory}");
        string wonOrLost = isVictory ? "won" : "lost";
        await Console.Out.WriteLineAsync($"You {wonOrLost} after {moveNumber} moves");
    }
}
