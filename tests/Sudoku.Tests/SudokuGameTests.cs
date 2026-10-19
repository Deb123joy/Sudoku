using Sudoku;
using Xunit;

namespace Sudoku.Tests;

public sealed class SudokuGameTests
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
        puzzle[1, 1] = 0;
        puzzle[2, 0] = 0;

        return new SudokuGame(puzzle, Solution, new Random(1));
    }

    [Fact]
    public void TrySet_AcceptsCorrectNumber()
    {
        var game = CreateGame();

        bool result = game.TrySet(0, 2, 4, out _);

        Assert.True(result);
        Assert.Equal(4, game.GetCell(0, 2));
    }

    [Fact]
    public void TrySet_RejectsIncorrectNumber()
    {
        var game = CreateGame();

        bool result = game.TrySet(0, 2, 1, out _);

        Assert.False(result);
        Assert.Equal(0, game.GetCell(0, 2));
    }

    [Fact]
    public void TrySet_RejectsNumberOutsideRange()
    {
        var game = CreateGame();

        Assert.False(game.TrySet(0, 2, 0, out _));
        Assert.False(game.TrySet(0, 2, 10, out _));
    }

    [Fact]
    public void TrySet_CannotModifyFixedCell()
    {
        var game = CreateGame();

        Assert.False(game.TrySet(0, 0, 5, out _));
        Assert.Equal(5, game.GetCell(0, 0));
    }

    [Fact]
    public void TryClear_ClearsEditableCell()
    {
        var game = CreateGame();
        game.TrySet(0, 2, 4, out _);

        Assert.True(game.TryClear(0, 2, out _));
        Assert.Equal(0, game.GetCell(0, 2));
    }

    [Fact]
    public void TryClear_CannotClearFixedCell()
    {
        var game = CreateGame();

        Assert.False(game.TryClear(0, 0, out _));
        Assert.Equal(5, game.GetCell(0, 0));
    }

    [Fact]
    public void TryHint_FillsAnEmptyCellCorrectly()
    {
        var game = CreateGame();

        Assert.True(game.TryHint(out _));
        Assert.True(game.IsValidGrid());
    }

    [Fact]
    public void IsValidGrid_ReturnsTrueForValidPartialGrid()
    {
        var game = CreateGame();

        Assert.True(game.IsValidGrid());
    }

    [Fact]
    public void IsComplete_ReturnsTrueForSolvedBoard()
    {
        var game = new SudokuGame(Solution, Solution);

        Assert.True(game.IsComplete());
    }

    [Fact]
    public void IsComplete_ReturnsFalseForIncompleteBoard()
    {
        var game = CreateGame();

        Assert.False(game.IsComplete());
    }
}
