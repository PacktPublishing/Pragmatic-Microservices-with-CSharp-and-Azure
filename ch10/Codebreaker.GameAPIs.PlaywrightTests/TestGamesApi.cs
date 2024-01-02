

[assembly: Category("SkipWhenLiveUnitTesting")]

namespace Codebreaker.Apis.IntegrationTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TestGamesApi : PlaywrightTest
{
    private readonly string _baseUrl = "http://localhost:9400";
    private IAPIRequestContext? _request = default;

    [SetUp]
    public async Task SetupApiTesting()
    {
        await CreateAPIRequestContext();
    }

    private async Task CreateAPIRequestContext()
    {
        // setup authentication when needed
        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = _baseUrl
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
    public async Task ShouldCreateA6x4GameAndReturnMoveResult()
    {
        if (_request is null)
        {
            Assert.Fail();
            return;
        }

        string playerName = "testuser";
        Guid id = await CreateGameAsync(playerName);

        await SetFirstMoveAsync(id, playerName);

        string[] codes = await GetTheGameAsync(id);

        // set the winning move
        await SetWinningMoveAsync(id, playerName, codes);

    }

    private async Task<Guid> CreateGameAsync(string playerName)
    {
        if (_request is null)
        {
            Assert.Fail();
            return Guid.Empty;
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
        Assert.That(response.Ok, Is.True);

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

        Assert.Multiple(() =>
        {
            // holes should be 4 with this game type
            Assert.That(numberCodes, Is.EqualTo(4));

            // max moves should be 12 with this game type
            Assert.That(maxMoves, Is.EqualTo(12));

            // username returned should contain the username requested
            Assert.That(json.Value.GetProperty("playerName").ToString(), Is.EqualTo(playerName));
        });

        return id;
    }

    private async Task SetFirstMoveAsync(Guid id, string playerName)
    {
        if (_request is null)
        {
            Assert.Fail();
            return;
        }

        Dictionary<string, object> request = new()
        {
            ["id"] = id.ToString(),
            ["gameType"] = "Game6x4",
            ["playerName"] = playerName,
            ["moveNumber"] = 1,
            ["guessPegs"] = new string[] { "Red", "Blue", "Red", "Blue" }
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
    }

    private async Task<string[]> GetTheGameAsync(Guid id)
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
            Assert.That(moveNumber, Is.EqualTo(1));
            Assert.That(codes.EnumerateArray().Count(), Is.EqualTo(4));
        });

        string[] codesArray = codes.Deserialize<string[]>()!; // codes is not null as it is checked above
        return codesArray;
    }

    private async Task SetWinningMoveAsync(Guid id, string playerName, string[] guesses)
    {
        if (_request is null)
        {
            Assert.Fail();
            return;
        }

        Dictionary<string, object> request = new()
        {
            ["id"] = id.ToString(),
            ["gameType"] = "Game6x4",
            ["playerName"] = playerName,
            ["moveNumber"] = 2,
            ["guessPegs"] = guesses
        };

        var response = await _request.PatchAsync($"{_baseUrl}/games/{id}", new()
        {
            DataObject = request
        });

        Assert.That(response.Ok, Is.True);

        var json = await response.JsonAsync();
        JsonElement results = json.Value.GetProperty("results");
        bool victory = bool.Parse(json.Value.GetProperty("isVictory").ToString());
        Assert.Multiple(() =>
        {
            Assert.That(results.EnumerateArray().Count(), Is.LessThanOrEqualTo(4));
            Assert.That(results.EnumerateArray().All(x => x.ToString() is "Black" or "White"));
            Assert.That(victory, Is.True);
        });
    }

}
