using System.Diagnostics;

namespace Codebreaker.GameAPIs.Client.Tests;

public class GamesClientTests
{
    [Fact]
    public async Task StartGameAsync_Should_ReturnResults()
    {
        // Arrange
        (var httpClient, var handlerMock) = GetHttpClientSkeleton();

        GamesClient gamesClient = new(httpClient, NullLogger<GamesClient>.Instance);

        // Act
        var (GameId, NumberCodes, MaxMoves, FieldValues) = await gamesClient.StartGameAsync(Models.GameType.Game6x4, "test");

        // Assert
        Assert.Equal(4, NumberCodes);
        Assert.Equal(12, MaxMoves);
        Assert.Single(FieldValues.Keys);
        Assert.Equal("colors", FieldValues.Keys.First());
        Assert.Equal(6, FieldValues["colors"].Length);

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task StartGameAsync_Should_StartGameAndTriggerGameCreatedEven()
    {
        // Arrange
        (var httpClient, var handlerMock) = GetHttpClientSkeleton();
        bool startActivityReceived = false;
        bool stopActivityReceived = false;

        using ActivityListener listener = new()
        {
            ShouldListenTo = _ => true,
            ActivityStarted = activity =>
            {
                if (activity.OperationName == "StartGameAsync")
                {
                    startActivityReceived = true;
                    Assert.Equal(ActivityKind.Client, activity.Kind);
                }
            },
            ActivityStopped = activity =>
            {
                if (activity.OperationName == "StartGameAsync")
                {
                    stopActivityReceived = true;
                    string? gameId = activity.GetBaggageItem("gameId");
                    Assert.NotNull(gameId);  // gameId needs to be part of the baggage
                    ActivityEvent? gameCreatedEvent = activity.Events.FirstOrDefault(e => e.Name == "GameCreated");
                    Assert.NotNull(gameCreatedEvent);
                    var tag = gameCreatedEvent.Value.Tags.FirstOrDefault(t => t.Key == "gameType");
                    Assert.Equal(tag.Value, "Game6x4");
   
                    Assert.Equal(ActivityKind.Client, activity.Kind);                  
                }
            },
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };

        ActivitySource.AddActivityListener(listener);
        
        GamesClient gamesClient = new(httpClient, NullLogger<GamesClient>.Instance);

        // Act
        var (GameId, NumberCodes, MaxMoves, FieldValues) = await gamesClient.StartGameAsync(Models.GameType.Game6x4, "test");

        Assert.True(startActivityReceived);
        Assert.True(stopActivityReceived);
    }

    private static (HttpClient Client, Mock<HttpMessageHandler> Handler) GetHttpClientSkeleton()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x[It.IsAny<string>()]).Returns("http://localhost:5000");

        Mock<HttpMessageHandler> handlerMock = new(MockBehavior.Strict);
        string returnMessage = """
        {
            "gameId": "af8dd39f-6e16-41ef-9155-dcd3cf081e87",
            "gameType": "Game6x4",
            "playerName": "test",
            "numberCodes": 4,
            "maxMoves": 12,
            "fieldValues": {
                "colors": [
                    "Red",
                    "Green",
                    "Blue",
                    "Yellow",
                    "Purple",
                    "Orange"
                ]
            }
        }
        """;
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(returnMessage)
            }).Verifiable();

        return (new(handlerMock.Object)
        {
            BaseAddress = new Uri(configMock.Object["GameAPIs"] ?? throw new InvalidOperationException())
        }, handlerMock);
    }
}
