using Codebreaker.GameAPIs.Client;
using Codebreaker.GameAPIs.Client.Models;

using Spectre.Console;

namespace Codebreaker.Client;
internal class Runner(GamesClient client)
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public async Task StartAsync()
    {
        GameType gameType = Inputs.GetGameType();
        string playerName = Inputs.GetPlayername();
        (Guid gameId, int numberCodes, int maxMoves, IDictionary<string, string[]>? fields) = await AnsiConsole.Status().StartAsync("Starting game...", async _ =>
        {
            (Guid gameId, int numberCodes, int maxMoves, var fields) = await client.StartGameAsync(gameType, playerName, _cancellationTokenSource.Token);
            return (gameId, numberCodes, maxMoves, fields);
        });
        // (Guid gameId, int numberCodes, int maxMoves, var fields) = await client.StartGameAsync(gameType, playerName, _cancellationTokenSource.Token);

        int moveNumber = 0;
        bool ended = false;
        bool isVictory = false;
        do
        {
            moveNumber++;
            string[] guesses = Inputs.GetFieldChoices(numberCodes, fields);
            Console.Write($"{moveNumber}. {string.Join(" ", guesses.Select(s => s.PadRight(8, ' ')))}");
            (string[] results, ended, isVictory) = await client.SetMoveAsync(gameId, moveNumber, guesses);
            Console.WriteLine($" ** {string.Join(' ', results)}");
        } while (!ended);
        Console.WriteLine($"Victory: {isVictory}");
        string wonOrLost = isVictory ? "won" : "lost";
        Console.WriteLine($"You {wonOrLost} after {moveNumber} moves");
    }

}
