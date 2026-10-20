# Sudoku Console Application

## 1. Overview

This project is a command-line Sudoku game developed in C# using .NET. It allows users to play a randomly generated Sudoku puzzle through a simple text-based interface.

The application generates a valid Sudoku solution, removes numbers to create a puzzle with exactly 30 pre-filled cells, and allows the player to enter numbers, clear cells, request hints, validate the grid, and quit the game.

The solution is designed with maintainability, readability, object-oriented programming, and testability in mind.


### Main Components


| `Program.cs` | Application entry point and object initialization |
| `SudokuGame.cs` | Maintains the board, validates moves, handles hints and detects completion |
| `SudokuGenerator.cs` | Generates Sudoku solutions and creates puzzles with unique solutions |

| `ConsoleGame.cs` | Displays the grid, processes user commands and manages the game loop |
| `SudokuGameTests.cs` | Tests the game rules and state management |
| `ConsoleGameTests.cs` | Tests command handling and console interactions |



## 2. Setup and Execution

### Clone or extract the project

Open a terminal in the project root directory, where `Sudoku.sln` is located.

### Build the application

```
dotnet build
```

### Run the application

```
dotnet run --project src/Sudoku
```

The application generates a puzzle and displays the Sudoku grid in the terminal.



## 3. Testing

The project uses xUnit for automated testing.

### Run all tests

From the solution root:

```
dotnet test
```



## 4. Design Principles

The application separates game rules, puzzle generation, and console interaction to keep each component focused on a single responsibility.

### Single Responsibility Principle

- `SudokuGame` manages game state and rules.
- `SudokuGenerator` generates puzzles.
- `ConsoleGame` manages user interaction.
- `Program` initializes and starts the application.

### Encapsulation

The game board and solution are maintained internally. Changes are performed through methods that validate moves and protect fixed cells.

### Dependency Injection

The console interface receives `TextReader` and `TextWriter` dependencies, allowing it to be tested with in-memory streams instead of a physical terminal.

### Separation of Concerns

The Sudoku logic does not depend on console input or output. This allows the game rules to be tested independently from the user interface.

### Maintainability

Small, focused methods make the code easier to read, test, debug, and extend.

## 5. Assumptions and Limitations

- The application is designed for a single player in a console environment.
- The board uses zero to represent empty cells internally.
- The puzzle generator uses randomized candidate ordering.
- Puzzle generation can fail to reach exactly 30 clues in a single attempt; retry logic can be added to improve resilience.
- A valid grid is not necessarily a complete solution; the game checks both validity and completion.
- The application validates entered numbers against the generated solution, so incorrect numbers are rejected immediately rather than retained as temporary player guesses.

