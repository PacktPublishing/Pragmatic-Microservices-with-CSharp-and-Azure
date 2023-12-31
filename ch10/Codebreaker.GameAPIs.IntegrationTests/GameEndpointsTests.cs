using System.Net;
using System.Net.Http.Json;

using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Tests;

public class GameEndpointsTests
{
    [Fact]
    public async Task SetMoveWithInvalidMoveNumberShouldReturnBadRequest()
    {
        await using GamesApiApplication app = new();
        var client = app.CreateClient();

        int moveNumber = 0;
        CreateGameResponse gameResponse = await StartGameFixtureAsync(app);

        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);       
    }

    [Fact]
    public async Task SetMoveWithInvalidGuessNumberShouldReturnBadRequest()
    {
        await using GamesApiApplication app = new();
        var client = app.CreateClient();

        int moveNumber = 1;
        CreateGameResponse gameResponse = await StartGameFixtureAsync(app);

        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red",]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    [Fact]
    public async Task SetMoveWithWrongGuessesShouldReturnBadRequest()
    {
        await using GamesApiApplication app = new();
        var client = app.CreateClient();

        int moveNumber = 1;
        CreateGameResponse gameResponse = await StartGameFixtureAsync(app);

        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Schwarz"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    private static async Task<CreateGameResponse> StartGameFixtureAsync(GamesApiApplication app)
    {
        var client = app.CreateClient();
        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await client.PostAsJsonAsync("/games", request);
        var gameReponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();

        if (gameReponse is null)
            Assert.Fail("gameResponse is null");
        return gameReponse;
    }
}
