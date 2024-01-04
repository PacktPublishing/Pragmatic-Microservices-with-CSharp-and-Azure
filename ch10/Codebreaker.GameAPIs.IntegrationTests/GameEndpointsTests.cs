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
        var response = await client.PatchAsJsonAsync(uri, updateGameRequest);
        var updateGameResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>();
        Assert.NotNull(updateGameResponse);

        // cheat to get the result
        if (!updateGameResponse.IsVictory)
        {
            Game? game = await client.GetFromJsonAsync<Game?>(uri);
            Assert.NotNull(game);

            // send the second move
            moveNumber = 2;
            updateGameRequest = new UpdateGameRequest(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
            {
                GuessPegs = game.Codes
            };
            response = await client.PatchAsJsonAsync(uri, updateGameRequest);

            // check the result
            Assert.True(response.IsSuccessStatusCode);
            updateGameResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>();

            Assert.NotNull(updateGameResponse);
            Assert.True(updateGameResponse.Ended);
            Assert.True(updateGameResponse.IsVictory);

        }
        // delete the game
        response = await client.DeleteAsync(uri);
        Assert.True(response.IsSuccessStatusCode);
    }

    private static async Task<(HttpClient Client, CreateGameResponse GameResponse)> StartGameFixtureAsync(GamesApiApplication app)
    {
        HttpClient client = app.CreateClient();
        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await client.PostAsJsonAsync("/games", request);
        Assert.True(response.IsSuccessStatusCode);

        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        return (client, gameResponse);
    }
}
