using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Codebreaker.GameAPIs.Client.Models;

using Microsoft.Extensions.Logging;

namespace Codebreaker.GameAPIs.Client;

/// <summary>
/// Client to interact with the Codebreaker Game API.
/// </summary>
public class GamesClient(HttpClient httpClient, ILogger<GamesClient> logger) : IGamesClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger _logger = logger;
    private readonly static JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Starts a new game
    /// </summary>
    /// <param name="gameType">The game type with one of the <see cref="GameType"/>enum values</param>
    /// <param name="playerName">The name of the player</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request early</param>
    /// <returns>A tuple with the unique game id, the number of codes that need to be filled, the maximum available moves, and possible field values for guesses</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="HttpRequestException"></exception>"
    public async Task<(Guid GameId, int NumberCodes, int MaxMoves, IDictionary<string, string[]> FieldValues)>
        StartGameAsync(GameType gameType, string playerName, CancellationToken cancellationToken = default)
    {
        try
        {
            CreateGameRequest createGameRequest = new(gameType, playerName);
            var response = await _httpClient.PostAsJsonAsync("/games", createGameRequest, s_jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>(s_jsonOptions, cancellationToken) ?? throw new InvalidOperationException();
            return (gameResponse.GameId, gameResponse.NumberCodes, gameResponse.MaxMoves, gameResponse.FieldValues);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "StartGameAsync error {error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Set a game move by supplying guess pegs. This method returns the results of the move (the key pegs), and whether the game ended, and whether the game was won.
    /// </summary>
    /// <param name="gameId">The game id received from StartGameAsync</param>
    /// <param name="playerName">The player name (needs to be the same as received). This must match with the game started.</param>
    /// <param name="gameType">The game type with one of the <see cref="GameType"/>enum values. This must match with the game started.</param>
    /// <param name="moveNumber">The incremented move number. The game analyzer returns an error if this does not match the state of the game.</param>
    /// <param name="guessPegs">The guess pegs for this move. The number of guess pegs must conform to the number codes returned when creating the game.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request early.</param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>"
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid gameId, string playerName, GameType gameType, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateGameRequest updateGameRequest = new(gameId, gameType, playerName, moveNumber)
            {
                GuessPegs = guessPegs
            };
            var response = await _httpClient.PatchAsJsonAsync($"/games/{gameId}", updateGameRequest, s_jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var moveResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>(s_jsonOptions, cancellationToken)
                ?? throw new InvalidOperationException();
            (_, _, _, bool ended, bool isVictory, string[] results) = moveResponse;
            return (results, ended, isVictory);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "SetMoveAsync error {error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a game by ID.
    /// </summary>
    /// <param name="gameId">The unique identifier of a game.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request early.</param>
    /// <returns>The <see cref="GameInfo"/> if it exists, otherwise null.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<GameInfo?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        GameInfo? game;
        try
        {
            game = await _httpClient.GetFromJsonAsync<GameInfo>($"/games/{gameId}", s_jsonOptions, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation(ex, "GetGameAsync game not found - {error}", ex.Message);
            return default;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "GetGameAsync error {error}", ex.Message);
            throw;
        }
        return game;
    }

    /// <summary>
    /// Retrieves a list of games matching a specified query.
    /// </summary>
    /// <param name="query">The games query object containing parameters to filter games.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request early.</param>
    /// <returns>An IEnumerable collection of Game objects that match the specified query.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<IEnumerable<GameInfo>> GetGamesAsync(GamesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<GameInfo> games = (await _httpClient.GetFromJsonAsync<IEnumerable<GameInfo>>($"/games/{query.AsUrlQuery()}", s_jsonOptions, cancellationToken)) ?? Enumerable.Empty<GameInfo>();
            return games;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "GetGamesAsync error {error}", ex.Message);
            throw;
        }
    }
}
