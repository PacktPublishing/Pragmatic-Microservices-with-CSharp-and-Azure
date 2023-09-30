# Create a game

```mermaid
sequenceDiagram
    participant GameEndpoints
    participant GamesService
    participant GamesFactory
    participant GamesSqlServerContext
    participant Sql Server
    GameEndpoints->>GamesService: StartGame
    GamesService->>GamesFactory: CreateGame
    GamesService->>GamesSqlServerContext: AddGame(Game)
    GamesSqlServerContext->>Sql Server: INSERT(Game)
    GamesService-->>GameEndpoints: Game with random codes
```