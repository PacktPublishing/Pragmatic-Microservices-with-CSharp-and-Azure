# Game Models

```mermaid
classDiagram
  direction TD
  Game <|-- GameT~TField, TResult~
  IGame~TField, TResult~ <|.. GameT~TField, TResult~
  IMove~TField, TResult~ <|.. MoveT~TField, TResult~
  Move <|-- MoveT~TField, TResult~
  IMove~TField, TResult~ "*" <-- "1" GameT~TField, TResult~ : Contains
  class IGame~TField, TResult~ {
    <<interface>>
  }
  class Game{
    <<abstract>>
    +gameId
    +gameType
    +playerName
  }
  class GameT~TField, TResult~{
    +fields
    +codes
    +results
    +moves
  }
  class IMove~TField, TResult~{
    <<interface>>
  }
  class Move{
    <<abstract>>
    +gameId
    +moveNumber
  }
  class MoveT~TField, TResult~{
    +guessPegs
    +keyPegs
  }

```
