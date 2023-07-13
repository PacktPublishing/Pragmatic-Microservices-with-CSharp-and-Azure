using Codebreaker.GameAPIs.Client.Models;

using Spectre.Console;

namespace Codebreaker.Client;

internal class Inputs
{
    public static GameType GetGameType() =>
        AnsiConsole.Prompt(new SelectionPrompt<GameType>()
                       .Title("Select the game type")
                       .AddChoices(Enum.GetValues<GameType>()));

    public static string GetPlayername() =>
        AnsiConsole.Ask<string>("Enter player name");

    public static (string Color, string? Shape) GetFieldChoice(string[] colors, string[]? shapes)
    {
        string color = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select the color")
            .AddChoices(colors));

        if (shapes is null)
        {
            return (color, null);
        }

        string shape = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select the shape")
            .AddChoices(shapes));
        return (color, shape);
    }

    public static string[] GetFieldChoices(int numberCodes, IDictionary<string, string[]> fieldValues)
    {
        List<string> selections = new();

        for (int i = 0; i < numberCodes; i++)
        {
            List<string> guessList = new();
            foreach (var key in fieldValues.Keys)
            {
                string guess = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title($"{key.ToLower()} {i + 1}")
                    .AddChoices(fieldValues[key]));
                guessList.Add(guess);
            }

            selections.Add(string.Join('#', guessList));
        }

        return selections.ToArray();
    }
}
