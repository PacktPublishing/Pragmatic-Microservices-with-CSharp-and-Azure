namespace Codebreaker.GameAPIs.Data.InMemory;

public partial class GamesMemoryRepository : IGamesRepository
{
	private static partial class Log
	{
		[LoggerMessage(
		    EventId = 8001,
		    Level = LogLevel.Error,
			Message = "Game {GameId} exists already!")]
		public static partial void GameExists(ILogger logger, Guid gameId);

        [LoggerMessage(
			EventId = 8002,
			Level = LogLevel.Warning,
			Message = "Game {GameId} not found")]
        public static partial void GameNotFound(ILogger logger, Guid gameId);
    }
}
