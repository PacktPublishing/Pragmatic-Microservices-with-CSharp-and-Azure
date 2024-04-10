```mermaid
sequenceDiagram
  participant bot-service
  participant game-apis
  participant live-service
  participant live-client

  bot-service->>game-apis: gRPC - start game
  loop repeat until complete
    bot-service->>game-apis: gRPC - set move
  end
  game-apis->>live-service: gRPC - game complete
  live-service->>live-client: signalR game complete
```
