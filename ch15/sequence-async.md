```mermaid
sequenceDiagram
  box
    participant bot-client
  end

  box
    participant bot-service
    participant game-apis
  end

  box
    participant ranking-service
    participant live-service
    participant live-client
  end

  bot-client-)bot-service: message: start games
  bot-service->>game-apis: gRPC - start game
  loop repeat until complete
    bot-service->>game-apis: gRPC - set move
  end
  par
    game-apis-)ranking-service: event - game complete
  and
    game-apis-)live-service: event game completed
  end
  live-service->>live-client: SignalR game complete
```
