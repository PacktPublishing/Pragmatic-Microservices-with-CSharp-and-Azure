using Microsoft.Extensions.Configuration;

[assembly: Category("SkipWhenLiveUnitTesting")]

namespace Codebreaker.APIs.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TestGamesApi : PlaywrightTest
{

    private string _baseUrl;
    private IAPIRequestContext? _request = default;

    [SetUp]
    public async Task SetupApiTesting()
    {
        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
        var config = configurationBuilder.Build();
        _baseUrl = config["BaseUrl"] ?? "abc";
        
        await CreateAPIRequestContext();
    }

    private async Task CreateAPIRequestContext()
    {
        // setup authentication when needed
        Dictionary<string, string> headers = new();
        headers.Add("Accept", "application/json");

        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = _baseUrl,
            ExtraHTTPHeaders = headers
        });
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        if (_request != null)
        {
            await _request.DisposeAsync();
        }
    }

    [Test]
    public async Task PlayTheGameToWin()
    {
        if (_request is null)
        {
            Assert.Fail();
            return;
        }

        string playerName = "test";
        (Guid id, string[] colors) = await CreateGameAsync(playerName);

        int moveNumber = 1;
        bool gameEnded = false;

        while (moveNumber < 10 && !gameEnded)
        {
            string[] guesses = Random.Shared.GetItems<string>(colors, 4).ToArray();
            gameEnded = await SetMoveAsync(id, playerName, moveNumber++, guesses);
        }

        if (!gameEnded)
        {
            string[] correctCodes = await GetTheGameAsync(id, moveNumber - 1);
            gameEnded = await SetMoveAsync(id, playerName, moveNumber++, correctCodes);
        }

        Assert.That(gameEnded, Is.True);
    }

    private async Task<(Guid Id, string[] Colors)> CreateGameAsync(string playerName)
    {
        if (_request is null)
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
        var response = await _request.PostAsync($"{_baseUrl}/games", new()
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
        if (_request is null)
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

        var response = await _request.PatchAsync($"{_baseUrl}/games/{id}", new()
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

    private async Task<string[]> GetTheGameAsync(Guid id, int expectedMoveNumber)
    {
        if (_request is null)
        {
            Assert.Fail();
            throw new InvalidOperationException();
        }

        var response = await _request.GetAsync($"{_baseUrl}/games/{id}");
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
}
