using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Codebreaker.GameAPIs.Client.Models;

namespace Codebreaker.GameAPIs.Client;
public class GamesClient
{
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
        CreateGameRequest createGameRequest = new(gameType, playerName);
        var response = await _httpClient.PostAsJsonAsync("/games", createGameRequest, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>(_jsonOptions, cancellationToken) ?? throw new InvalidOperationException();
        return (gameResponse.GameId, gameResponse.NumberCodes, gameResponse.MaxMoves, gameResponse.FieldValues); 
    }

    public async Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid gameId, string playerName, GameType gameType, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default)
    {
        UpdateGameRequest updateGameRequest = new(gameId, gameType, playerName, moveNumber)
        {
            GuessPegs = guessPegs
        };
        var response = await _httpClient.PatchAsJsonAsync($"/games/{gameId}", updateGameRequest, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        var moveResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>(_jsonOptions, cancellationToken) 
            ?? throw new InvalidOperationException();
        (_, _, _, bool ended, bool isVictory, string[] results) = moveResponse;
        return (results, ended, isVictory);
    }

    public async Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        Game? game = default;
        try
        {
            game = await _httpClient.GetFromJsonAsync<Game>($"/games/{gameId}", _jsonOptions, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = (await _httpClient.GetFromJsonAsync<IEnumerable<Game>>($"/games/{query.AsUrlQuery()}", _jsonOptions, cancellationToken)) ?? Enumerable.Empty<Game>();
        return games;
    }

    public async Task DeleteGameAsync(Guid gameId)
    {
        var response = await _httpClient.DeleteAsync($"/games/{gameId}");
    }
}
