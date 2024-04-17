# CNinnovation.Codebreaker.Analyzers

This library contains game move analyzers for the Codebreaker app. Reference this library when creating a Codebreaker service.
See https://github.com/codebreakerapp for more information on the complete solution.

See [Codebreakerlight](https://github.com/codebreakerapp/codebreakerlight) for a simple version of the Codebreaker solution with a Wiki to create your own Codebreaker service.

## Types available in this package

### Contracts, namespace Codebreaker.GameAPIs.Contracts

| Type     | Description        |
|----------|--------------------|
| IGame    | Implement this interface with your game model. This is required by the analyzer |
| IGameGuessAnalyzer | If you want to create your own game type, create an analyzer implementing this interface. Instead, you can derive your analyzer type from the base class `GameGuessAnalyzer` |

### Analyzers, namespace Codebreaker.GameAPIs.Analyzers

| Type     | Description        |
|----------|--------------------|
| GameGuessAnalyzer  | This is the base class of all analyzers. Derive from this class when you create your own game type |
| ColorGameGuessAnalyzer | This analyzer uses the types `ColorField` and `ColorResult` to analyze games moves with a list of colors. |
| SimpleGameGuessAnalyzer | This analyzer implements the children-mode of the game and uses the types `ColorField` and `SimpleColorResult` to analyze games moves with a list of colors. |
| ShapeGameGuessAnalyzer | This analyzer uses the types `ShapeAndColorField` and `ShapeAndColorResult` to analyze games moves with a list of shapes and colors. |

### Field and Result Types, namespace Codebreaker.GameAPIs.Models

| Type     | Description        |
|----------|--------------------|
| GameTypes | Constants for available game types. |
| ColorField  | A field type for color fields |
| ShapeAndColorField | A field type for shape and color fields |
| ColorResult | A result type with `Correct` and `WrongPosition` numbers |
| ShapeAndColorResult | A result type with `Correct`, `WrongPosition` and `ColorOrShape` (either the color or the shape is correct) numbers |
| SimpleColorResult | A result type with a list to show positional results using the `ResultValue` enum. |
