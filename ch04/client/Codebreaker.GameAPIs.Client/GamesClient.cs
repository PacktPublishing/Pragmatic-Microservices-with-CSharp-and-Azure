using System.Net.Http.Json;
using System.Text.Json;

using Codebreaker.GameAPIs.Client.Models;

namespace Codebreaker.GameAPIs.Client;
public class GamesClient
{
    private GameType _gameType;
    private Guid _gameId;
    private string? _playerName;

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public GamesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
   
    public async Task<(Guid GameId, int numberCodes, int maxMoves, IDictionary<string, string[]> FieldValues)> 
        StartGameAsync(GameType gameType, string playerName, CancellationToken cancellationToken = default)
    {
        _gameType = gameType;
        _playerName = playerName;

        CreateGameRequest createGameRequest = new(_gameType, _playerName);
        var response = await _httpClient.PostAsJsonAsync("/games", createGameRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>(_jsonOptions, cancellationToken) ?? throw new InvalidOperationException();
        _gameId = gameResponse.GameId;
        _gameType = gameResponse.GameType;
        return (gameResponse.GameId, gameResponse.NumberCodes, gameResponse.MaxMoves, gameResponse.FieldValues); 
    }

    public async Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid gameId, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default)
    {
        if (_playerName is null)
            throw new InvalidOperationException();

        UpdateGameRequest updateGameRequest = new(_gameId, _gameType, _playerName, moveNumber)
        {
            GuessPegs = guessPegs
        };
        var response = await _httpClient.PatchAsJsonAsync($"/games/{_gameId}", updateGameRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var moveResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>(_jsonOptions, cancellationToken) 
            ?? throw new InvalidOperationException();
        (_, _, _, bool ended, bool isVictory, string[] results) = moveResponse;
        return (results, ended, isVictory);
    }

    public async Task<Game?> GetGameAsync(Guid gameId)
    {
        Game? game = default;
        try
        {
            game = await _httpClient.GetFromJsonAsync<Game>($"{gameId}", _jsonOptions);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery query)
    {
        IEnumerable<Game> games = (await _httpClient.GetFromJsonAsync<IEnumerable<Game>>(query.AsUrlQuery(), _jsonOptions)) ?? Enumerable.Empty<Game>();
        return games;
    }

    public async Task DeleteGameAsync(Guid gameId)
    {
        var response = await _httpClient.DeleteAsync("gameId");
    }
}
