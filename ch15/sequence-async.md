# Sequence Diagrams - Asynchronous Communication

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

## Part 1 Bot to game APIs

```mermaid
%%{
  init: {
      "sequence": {
        "actorFontWeight": "bold",
        "actorFontSize": "24",
        "font-size": "44px"
        "noteFontSize": "24"
        ".labelText" {
          "font-size": "44px"
  
        }
      }
  }
}%%

sequenceDiagram
  participant bot client
  participant QUEUE
  participant bot service
  participant game APIs

  bot service->>QUEUE: wait for<br> messages

  bot client->>QUEUE: start games
  Note over bot client,QUEUE: send message

  QUEUE-)bot service: start games
  Note over QUEUE,bot service: receive message

  loop repeat until<br>games complete
    bot service->>game APIs: start game
    Note over bot service,game APIs: gRPC 

    loop repeat until<br>game ends
      bot service->>game APIs: set move
    end
  end

```

## Part 2 game APIs to live and ranking services

```mermaid
%%{
  init: {
      "sequence": {
        "actorFontWeight": "bold"
        "noteFontWeight": "bold"
      }
  }
}%%

sequenceDiagram
  participant game APIs
  participant EVENT HUB
  participant ranking service
  participant live service
  participant live client

  ranking service->>EVENT HUB: subscribe
  live service->>EVENT HUB: subscribe
  game APIs->>EVENT HUB: game ended
  Note over game APIs,EVENT HUB: Push event
  EVENT HUB-)ranking service: game ended
  Note over EVENT HUB,ranking service: Receive event
  EVENT HUB-)live service: game ended
  Note over EVENT HUB,live service: Receive event
  live service->>live client: game ended
  Note over live service,live client: SignalR

```