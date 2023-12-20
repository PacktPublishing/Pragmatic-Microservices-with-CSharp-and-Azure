# CNinnovation.Codebreaker.ViewModels

This library contains view-model types for XAML-based applications (WinUI, WPF, .NET MAUI...) to create Codebreaker games.

It is part of the Codebreaker solution.

See https://github.com/codebreakerapp for more information on the complete solution.

## The ViewModels

| Class | Description |
|-------|-------------|
| GamePageViewModel | The GamePageViewModel is the view-model type for the game page with commands to start games, set moves. |
| GameViewModel | The GameViewModel is the view-model type for game information. |
| MoveViewModel | The MoveViewModel is the view-model type for a game move. |
| InfoMessageViewModel | Alternative option of IDialogService |

The `GamePageViewModel` is the main view-model type to communicate with the application.

| Members     | Description        |
|------------|--------------------|
| ctor | Needs `IGamesClient` (communication with the games-service API), `IOptions<GamePageViewModelOptions>`, `IDialogService` |

### Supporting types

| Class | Description |
|-------|-------------|
| GameMode | An enumeration - is the game not running, started, are moves set, lost, won? |

In the constructor, inject the `HttpClient` class. You can use `Microsoft.Extensions.Http` to configure the `HttpClient` class.

## Model types

The following model types are used to return information about the game.

| Model type | Description |
|------------|-------------|
| Game | Contains the game id, the game status, the game moves and the game result |
| Move | Contains the move number, the guess (`GuessPegs`) and the result of the guess (`KeyPegs`) |
