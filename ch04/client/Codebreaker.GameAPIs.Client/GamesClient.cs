namespace Codebreaker.GameAPIs.Client;
public class GamesClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Game? game;
        try
        {
            game = await _httpClient.GetFromJsonAsync<Game>($"/games/{id}", _jsonOptions, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> games = (await _httpClient.GetFromJsonAsync<IEnumerable<Game>>($"/games/{query.AsUrlQuery()}", _jsonOptions, cancellationToken)) ?? [];
        return games;
    }

    public async Task<(Guid Id, int NumberCodes, int MaxMoves, IDictionary<string, string[]> FieldValues)>
        StartGameAsync(GameType gameType, string playerName, CancellationToken cancellationToken = default)
    {
        CreateGameRequest createGameRequest = new(gameType, playerName);
        var response = await _httpClient.PostAsJsonAsync("/games", createGameRequest, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>(_jsonOptions, cancellationToken) ?? throw new InvalidOperationException();
        return (gameResponse.Id, gameResponse.NumberCodes, gameResponse.MaxMoves, gameResponse.FieldValues);
    }

    public async Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid id, string playerName, GameType gameType, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default)
    {
        UpdateGameRequest updateGameRequest = new(id, gameType, playerName, moveNumber)
        {
            GuessPegs = guessPegs
        };
        var response = await _httpClient.PatchAsJsonAsync($"/games/{id}", updateGameRequest, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        var moveResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>(_jsonOptions, cancellationToken)
            ?? throw new InvalidOperationException();
        (_, _, _, bool ended, bool isVictory, string[] results) = moveResponse;
        return (results, ended, isVictory);
    }

    public async Task DeleteGameAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/games/{id}");
    }
}
