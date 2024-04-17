[assembly: Category("SkipWhenLiveUnitTesting")]
[assembly: LevelOfParallelism(2000)]

namespace Codebreaker.APIs.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
public class GamesApiTests : PlaywrightTest
{
    private IAPIRequestContext? _requestContext;
    private int _delayMoveMS = 1000;
    private string _playerName = "Test";

    [SetUp]
    public async Task SetupAPITestingAsync()
    {
        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
        var config = configurationBuilder.Build();

        _delayMoveMS = int.Parse(config["DelayMoveMS"] ?? "1000");
        _playerName = config["PlayerName"] ?? "Test";

        Dictionary<string, string> headers = new()
        {
            { "Accept", "application/json" }
        };

        _requestContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = config["BaseUrl"] ?? "http://localhost",
            ExtraHTTPHeaders = headers
        });
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        if (_requestContext != null)
        {
            await _requestContext.DisposeAsync();
        }
    }

    [Test]
    [Repeat(50)]
    public async Task PlayTheGameToWinAsync()
    {
        if (_requestContext is null)
        {
            Assert.Fail();
            return;
        }

        (Guid id, string[] colors) = await CreateGameAsync(_playerName);

        int moveNumber = 1;
        bool gameEnded = false;

        while (moveNumber < 10 && !gameEnded)
        {
            await Task.Delay(_delayMoveMS);
            string[] guesses = Random.Shared.GetItems<string>(colors, 4).ToArray();
            gameEnded = await SetMoveAsync(id, _playerName, moveNumber++, guesses);
        }

        if (!gameEnded)
        {
            await Task.Delay(_delayMoveMS);
            string[] correctCodes = await GetGameAsync(id, moveNumber - 1);
            gameEnded = await SetMoveAsync(id, _playerName, moveNumber++, correctCodes);
        }

        Assert.That(gameEnded, Is.True);
    }

    [Test]
    [Repeat(5)]
    public async Task PlayTheGameToEnd()
    {
        if (_requestContext is null)
        {
            Assert.Fail();
            return;
        }

        (Guid id, string[] colors) = await CreateGameAsync(_playerName);

        int moveNumber = 1;
        bool gameEnded = false;

        // the service should end the game after 12 moves
        while (!gameEnded)
        {
            await Task.Delay(_delayMoveMS);
            string[] guesses = Random.Shared.GetItems<string>(colors, 4).ToArray();
            gameEnded = await SetMoveAsync(id, _playerName, moveNumber++, guesses);
        }

        Assert.That(gameEnded, Is.True);
        Assert.That(moveNumber, Is.LessThanOrEqualTo(13));

        await DeleteGameAsync(id);
    }

    private async Task<(Guid Id, string[] Colors)> CreateGameAsync(string playerName)
    {
        if (_requestContext is null)
        {
            Assert.Fail();
            return (Guid.Empty, []);
        }

        Dictionary<string, string> request = new()
        {
            ["gameType"] = "Game6x4",
            ["playerName"] = playerName
        };

        // create a game
        var response = await _requestContext.PostAsync($"/games", new()
        {
            DataObject = request
        });
        Assert.That(response.Ok, Is.True);  // Playwright Ok is 200-299

        var json = await response.JsonAsync();
        Assert.DoesNotThrow(() =>
        {
            json.Value.GetProperty("gameType");
            json.Value.GetProperty("fieldValues");
        });

        // is the game type the same as requested?
        Assert.That(json.Value.GetProperty("gameType").ToString(), Is.EqualTo("Game6x4"));

        Guid id = json.Value.GetProperty("id").GetGuid();

        int numberCodes = int.Parse(json.Value.GetProperty("numberCodes").ToString());

        int maxMoves = int.Parse(json.Value.GetProperty("maxMoves").ToString());

        string[] colorFields = json.Value.GetProperty("fieldValues").GetProperty("colors").EnumerateArray().Select(color => color.ToString()).ToArray();

        Assert.Multiple(() =>
        {
            // codes should be 4 with this game type
            Assert.That(numberCodes, Is.EqualTo(4));

            // max moves should be 12 with this game type
            Assert.That(maxMoves, Is.EqualTo(12));

            // username returned should contain the username requested
            Assert.That(json.Value.GetProperty("playerName").ToString(), Is.EqualTo(playerName));

            // this game containers colors
            Assert.That(json.Value.GetProperty("fieldValues").GetProperty("colors").EnumerateArray().Count(), Is.EqualTo(6));
        });

        return (id, colorFields);
    }

    private async Task<bool> SetMoveAsync(Guid id, string playerName, int moveNumber, string[] guesses)
    {
        if (_requestContext is null)
        {
            Assert.Fail();
            return false;
        }

        Dictionary<string, object> request = new()
        {
            ["id"] = id.ToString(),
            ["gameType"] = "Game6x4",
            ["playerName"] = playerName,
            ["moveNumber"] = moveNumber,
            ["guessPegs"] = guesses
        };

        var response = await _requestContext.PatchAsync($"/games/{id}", new()
        {
            DataObject = request
        });

        Assert.That(response.Ok, Is.True);

        var json = await response.JsonAsync();
        JsonElement results = json.Value.GetProperty("results");
        Assert.Multiple(() =>
        {
            Assert.That(results.EnumerateArray().Count(), Is.LessThanOrEqualTo(4));
            Assert.That(results.EnumerateArray().All(x => x.ToString() is "Black" or "White"));
        });

        bool hasEnded = bool.Parse(json.Value.GetProperty("ended").ToString());
        return hasEnded;
    }

    private async Task<string[]> GetGameAsync(Guid id, int expectedMoveNumber)
    {
        if (_requestContext is null)
        {
            Assert.Fail();
            throw new InvalidOperationException();
        }

        var response = await _requestContext.GetAsync($"/games/{id}");
        var json = await response.JsonAsync();
        int moveNumber = int.Parse(json.Value.GetProperty("lastMoveNumber").ToString());
        bool victory = bool.Parse(json.Value.GetProperty("isVictory").ToString());
        JsonElement codes = json.Value.GetProperty("codes");

        Assert.Multiple(() =>
        {
            Assert.That(moveNumber, Is.EqualTo(expectedMoveNumber));
            Assert.That(codes.EnumerateArray().Count(), Is.EqualTo(4));
        });

        string[] codesArray = codes.Deserialize<string[]>()!; // codes is not null as it is checked above
        return codesArray;
    }

    private async Task DeleteGameAsync(Guid id)
    {
        if (_requestContext is null)
        {
            Assert.Fail();
            throw new InvalidOperationException();
        }

        var response = await _requestContext.DeleteAsync($"/games/{id}");

        Assert.That(response.Ok, Is.True);
    }
}
