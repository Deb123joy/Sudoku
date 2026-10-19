using Sudoku;
using Xunit;

namespace Sudoku.Tests;

public sealed class ConsoleGameTests
{
    private static readonly int[,] Solution =
    {
        { 5,3,4,6,7,8,9,1,2 },
        { 6,7,2,1,9,5,3,4,8 },
        { 1,9,8,3,4,2,5,6,7 },
        { 8,5,9,7,6,1,4,2,3 },
        { 4,2,6,8,5,3,7,9,1 },
        { 7,1,3,9,2,4,8,5,6 },
        { 9,6,1,5,3,7,2,8,4 },
        { 2,8,7,4,1,9,6,3,5 },
        { 3,4,5,2,8,6,1,7,9 }
    };

    private static SudokuGame CreateGame()
    {
        var puzzle = (int[,])Solution.Clone();
        puzzle[0, 2] = 0;

        return new SudokuGame(puzzle, Solution);
    }

    [Fact]
    public void Run_ProcessesMoveAndQuit()
    {
        var game = CreateGame();
        using var input = new StringReader("A3 4\nquit\n");
        using var output = new StringWriter();

        new ConsoleGame(game, input, output).Run();

        Assert.Equal(4, game.GetCell(0, 2));
        Assert.Contains("Number placed successfully", output.ToString());
        Assert.Contains("Goodbye!", output.ToString());
    }

    [Fact]
    public void Run_RejectsModificationOfFixedCell()
    {
        var game = CreateGame();
        using var input = new StringReader("A1 5\nquit\n");
        using var output = new StringWriter();

        new ConsoleGame(game, input, output).Run();

        Assert.Equal(5, game.GetCell(0, 0));
        Assert.Contains("pre-filled cell", output.ToString());
    }

    [Fact]
    public void Run_RecognizesSolvedPuzzle()
    {
        var game = CreateGame();
        using var input = new StringReader("A3 4\n");
        using var output = new StringWriter();

        new ConsoleGame(game, input, output).Run();

        Assert.True(game.IsComplete());
        Assert.Contains("Puzzle solved", output.ToString());
    }

    [Fact]
    public void Run_SupportsClearCommand()
    {
        var game = CreateGame();
        game.TrySet(0, 2, 4, out _);

        using var input = new StringReader("A3 clear\nquit\n");
        using var output = new StringWriter();

        new ConsoleGame(game, input, output).Run();

        Assert.Equal(0, game.GetCell(0, 2));
        Assert.Contains("Cell cleared", output.ToString());
    }
}
