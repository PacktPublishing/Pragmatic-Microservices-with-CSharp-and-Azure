using Aspire.Hosting;

using Codebreaker.GameAPIs.Models;

using System.Net;
using System.Net.Http.Json;

namespace Codebreaker.IntegrationTests.Tests;

public class GameAPIsTests : IAsyncLifetime
{
    private DistributedApplication? _app;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Codebreaker_AppHost>();        
        _app = await appHost.BuildAsync();
        await _app.StartAsync();
        _client = _app.CreateHttpClient("gameapis");
    }

    public async Task DisposeAsync()
    {
        if (_app is null) throw new InvalidOperationException();
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task SetMove_Should_ReturnBadRequest_WithInvalidMoveNumber()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        int moveNumber = 0;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await _client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    [Fact]
    public async Task SetMove_Should_ReturnBadRequest_WithInvalidGuessCount()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await _client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    [Fact]
    public async Task SetMove_Should_ReturnBadRequest_WithWrongGuesses()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Schwarz"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        var updateGameResponse = await _client.PatchAsJsonAsync(uri, updateGameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, updateGameResponse.StatusCode);
    }

    [Fact]
    public async Task SetMoves_Should_WinAGame()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        // send the first move
        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        response = await _client.PatchAsJsonAsync(uri, updateGameRequest);
        var updateGameResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>();
        Assert.NotNull(updateGameResponse);

        // cheat to get the result
        if (!updateGameResponse.IsVictory)
        {
            Game? game = await _client.GetFromJsonAsync<Game?>(uri);
            Assert.NotNull(game);

            // send the second move
            moveNumber = 2;
            updateGameRequest = new UpdateGameRequest(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
            {
                GuessPegs = game.Codes
            };
            response = await _client.PatchAsJsonAsync(uri, updateGameRequest);

            // check the result
            Assert.True(response.IsSuccessStatusCode);
            updateGameResponse = await response.Content.ReadFromJsonAsync<UpdateGameResponse>();

            Assert.NotNull(updateGameResponse);
            Assert.True(updateGameResponse.Ended);
            Assert.True(updateGameResponse.IsVictory);

        }
        // delete the game
        response = await _client.DeleteAsync(uri);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateGame_Should_ReturnCreated()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetGame_Should_ReturnOk()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        string uri = $"/games/{gameResponse.Id}";
        response = await _client.GetAsync(uri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SetMove_Should_ReturnOk()
    {
        if (_client is null) throw new InvalidOperationException();

        CreateGameRequest request = new(GameType.Game6x4, "test");
        var response = await _client.PostAsJsonAsync("/games", request);
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        Assert.NotNull(gameResponse);

        int moveNumber = 1;
        UpdateGameRequest updateGameRequest = new(gameResponse.Id, gameResponse.GameType, gameResponse.PlayerName, moveNumber)
        {
            GuessPegs = ["Red", "Red", "Red", "Red"]
        };

        string uri = $"/games/{updateGameRequest.Id}";
        response = await _client.PatchAsJsonAsync(uri, updateGameRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}