using Codebreaker.GameAPIs.Client;
using Codebreaker.GameAPIs.Client.Models;
using CodeBreaker.Blazor.Components;
using CodeBreaker.Blazor.Resources;
using CodeBreaker.Blazor.UI.Models.DataGrid;
using CodeBreaker.Blazor.UI.Services.Dialog;
using CodeBreaker.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CodeBreaker.Blazor.Pages;

public partial class ReportsPage
{
    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private IGamesClient GameClient { get; set; } = default!;

    [Inject]
    private ILogger<ReportsPage> Logger { get; set; } = default!;
    [Inject]
    private IStringLocalizer<Resource> Loc { get; set; } = default!;


    private List<GameInfo> _games = [];
    private bool _isLoadingGames = false;
    private DateOnly? _test;

    private List<string> Headers => [.. Loc.GetString("Reports_Table_Headers").Value.Split(",")];

    private readonly List<CodeBreakerColumnDefinition<GameInfo>> _columns = [
        new CodeBreakerColumnDefinition<GameInfo>("Gamername", game => game.PlayerName, true),
        new CodeBreakerColumnDefinition<GameInfo>("Start", game => game.StartTime, false),
        new CodeBreakerColumnDefinition<GameInfo>("End", game => game.EndTime.HasValue ? game.EndTime.Value : "----", false),
        new CodeBreakerColumnDefinition<GameInfo>("Number of Moves", game => game.Moves.Count(), true)
    ];

    public async Task GetGamesAsync()
    {
        Logger?.LogInformation("Calling GetReport for {date}", _test);
        _games = [];
        _isLoadingGames = true;
        try
        {
            var query = new GamesQuery(Date: _test, Ended: true);
            var response = await GameClient.GetGamesAsync(query);
            Logger?.LogDebug("Got response: {response}", response);
            _games = [..response ?? []];
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error calling GetGames");
            //TODO: handle Exception;
        }
        finally
        {
            _isLoadingGames = false;
        }
    }

    private void ShowReportDialog(GameInfo game)
    {
        var title = game.PlayerName;
        if (!game.EndTime.HasValue)
        {
            title = $"{title}: Game was canceled.";
        }
        else if (game.Moves.Any())
        {
            var diff = game.Moves.Last().GuessPegs.Except(game.Codes);
            title = diff.Any()
                ? $"{title}: Game was lost."
                : $"{title}: Game was won.";
        }

        DialogService.ShowDialog(new DialogContext(typeof(Playground), new Dictionary<string, object>
            {
                { nameof(Playground.Game), game },
                { nameof(Playground.GameFinished), true },
            }, title, []));
    }
}
