```mermaid
sequenceDiagram
  participant bot-client
  participant bot-service
  participant game-apis
  participant live-service
  participant live-client
  participant ranking-service

  bot-client->>bot-service: QUEUE - start games
  bot-service->>game-apis: gRPC - start game
  loop repeat until complete
    bot-service->>game-apis: gRPC - set move
  end
  game-apis->>live-service: EVENT - game complete
  live-service->>live-client: SignalR game complete
  game-apis->>ranking-service: EVENT - game complete
```
