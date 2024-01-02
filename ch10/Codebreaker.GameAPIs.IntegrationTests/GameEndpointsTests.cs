namespace Codebreaker.GameAPIs.Tests;

public class GameEndpointsTests
{
    [Fact]
    public async Task SetMove_Should_ReturnBadRequest_WithInvalidMoveNumber()
    {
        await using GamesApiApplication app = new();
        (HttpClient client, CreateGameResponse gameResponse) = await StartGameFixtureAsync(app);

        int moveNumber = 0;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);       
    }

    [Fact]
    public async Task SetMove_Should_ReturnBadRequest_WithInvalidGuessCount()
    {
        await using GamesApiApplication app = new();
        (HttpClient client, CreateGameResponse gameResponse) = await StartGameFixtureAsync(app);

        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    [Fact]
    public async Task SetMove_Should_ReturnBadRequest_WithWrongGuesses()
    {
        await using GamesApiApplication app = new();
        (HttpClient client, CreateGameResponse gameResponse) = await StartGameFixtureAsync(app);

        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Schwarz"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    [Fact]
    public async Task SetMoves_Should_WinAGame()
    {
        await using GamesApiApplication app = new();
        (HttpClient client, CreateGameResponse gameResponse) = await StartGameFixtureAsync(app);

        // send the first move
        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        // cheat to get the result

        var getGameResponse = await client.GetAsync(uri);

        // send the second move
        moveNumber = 2;
        updateGameRequest = new UpdateGameRequest(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {

        };

        // check the result

        // delete the game
        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    private static async Task<(HttpClient Client, CreateGameResponse Response)> StartGameFixtureAsync(GamesApiApplication app)
    {
        HttpClient client = app.CreateClient();
        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await client.PostAsJsonAsync("/games", request);
        Assert.True(response.IsSuccessStatusCode);

        var gameReponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameReponse);

        return (client, gameReponse);
    }
}
