using System.ComponentModel.DataAnnotations;

namespace Sudoku;
/// <summary>
/// Encapsulates a Sudoku puzzle, its solution, and operations for
/// interacting with the puzzle (setting/clearing cells, hints, and validation).
/// </summary>

public sealed class SudokuGame
{
   public const int Size = 9;// The width and height of the Sudoku grid.
                             
    public const int ClueCount = 30;//The number of pre-filled clues required in a generated puzzle.

    private readonly int[,] _solution;// Stores the completed solution used to validate player moves.
    
    private readonly bool[,] _fixedCells;// Identifies cells that were pre-filled and cannot be changed.
    private readonly int[,] _cells;//Stores the current state of the puzzle, including player moves. A value of 0 represents an empty cell.
    private readonly Random _random;//Generates random values for selecting hint cells.
    //Constructor: initializes and validates the puzzle.
    public SudokuGame(
        int[,] puzzle,
        int[,] solution,
        Random? random = null)
    {
        ArgumentNullException.ThrowIfNull(puzzle);
        ArgumentNullException.ThrowIfNull(solution);

        if (puzzle.GetLength(0) != Size ||
            puzzle.GetLength(1) != Size ||
            solution.GetLength(0) != Size ||
            solution.GetLength(1) != Size)
        {
            throw new ArgumentException(
                "Puzzle and solution must both be 9x9.");
        }

        _cells = (int[,])puzzle.Clone();
        _solution = (int[,])solution.Clone();
        _fixedCells = new bool[Size, Size];
        _random = random ?? Random.Shared;

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            { // Current value in the puzzle.
                int value = _cells[row, col];
                // Correct solution value for the current cell.
                int answer = _solution[row, col];

                if (value is < 0 or > 9 ||
                    answer is < 1 or > 9)
                {
                    throw new ArgumentException(
                        "Cell values must be between 0 and 9, " +
                        "and solutions must contain digits 1-9.");
                }

                if (value != 0 && value != answer)
                {
                    throw new ArgumentException(
                        "Puzzle clues must match the solution.");
                }
                // A non-zero puzzle value is a pre-filled clue.
                _fixedCells[row, col] = value != 0;
            }
        }

        if (!IsValidGrid(_solution) ||
            !IsValidGrid(_cells))
        {
            throw new ArgumentException(
                "The puzzle and solution must obey Sudoku rules.");
        }
    }
    /// <summary>
    /// Returns the current value of the cell at the given zero-based
    /// row and column. Throws if coordinates are out of range.
    /// </summary>
    public int GetCell(int row, int col)
    {
        CheckCoordinates(row, col);
        return _cells[row, col];
    }
    /// <summary>
    /// Returns true if the cell at the given coordinates is a fixed clue
    /// (pre-filled) and therefore cannot be changed by the player.
    /// </summary>
    public bool IsFixed(int row, int col)
    {
        CheckCoordinates(row, col);
        return _fixedCells[row, col];
    }
    /// <summary>
    /// Attempts to place <paramref name="number"/> into the cell at the
    /// specified zero-based <paramref name="row"/> and <paramref name="col"/>.
    /// On failure returns false and sets <paramref name="message"/> describing why.
    /// </summary>
    public bool TrySet(
        int row, int col, int number, out string message)
    {
        CheckCoordinates(row, col);
        string coord = $"{(char)('A' + row)}{col + 1}";

        if (_fixedCells[row, col])
        {
            message = $"Invalid move.{coord} is pre-filled. ";
            return false;
        }

        if (number is < 1 or > 9)
        {
            message = "The number must be between 1 and 9.";
            return false;
        }

        if (HasConflict(row, col, number))
        {
            message = "That number conflicts with its row, " +
                      "column, or 3x3 subgrid.";
            return false;
        }

        if (_solution[row, col] != number)
        {
            message = "Incorrect number. Try again.";
            return false;
        }

        _cells[row, col] = number;
        message = "Move accepted.";
        return true;
    }
    /// <summary>
    /// Attempts to clear (set to 0) the cell at the given coordinates.
    /// Returns false if the cell is fixed/clue and cannot be cleared.
    /// </summary>
    public bool TryClear(
        int row, int col, out string message)
    {
        CheckCoordinates(row, col);

        if (_fixedCells[row, col])
        {
            message = "You cannot clear a pre-filled cell.";
            return false;
        }

        _cells[row, col] = 0;
        message = "Cell cleared.";
        return true;
    }
    /// <summary>
    /// Provides a hint by filling one random empty cell with its solution value.
    /// Returns the coordinate and value in <paramref name="message"/>.
    /// </summary>
    public bool TryHint(out string message)
    {
        var emptyCells = new List<(int Row, int Col)>();

        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                if (_cells[row, col] == 0)
                    emptyCells.Add((row, col));
            }
        }

        if (emptyCells.Count == 0)
        {
            message = "There are no empty cells.";
            return false;
        }

        var cell = emptyCells[_random.Next(emptyCells.Count)];
        _cells[cell.Row, cell.Col] =
            _solution[cell.Row, cell.Col];

        message = $"Hint: {(char)('A' + cell.Row)}" +
                  $"{cell.Col + 1} = {_cells[cell.Row, cell.Col]}";

        return true;
    }
    /// <summary>
    /// Validates the current working grid for Sudoku rule compliance.
    /// </summary>
    public bool IsValidGrid()
    {
        return IsValidGrid(_cells);
    }
    /// <summary>
    /// Returns true when all cells are filled and the grid is valid.
    /// </summary>
    public bool IsComplete()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                if (_cells[row, col] == 0)
                    return false;
            }
        }

        return IsValidGrid(_cells);
    }
    /// <summary>
    /// Checks whether placing <paramref name="number"/> at the given
    /// coordinates would conflict with an existing number in the same
    /// row, column, or 3x3 subgrid.
    /// </summary>
    private bool HasConflict(int row, int col, int number)
    {
        for (int i = 0; i < Size; i++)
        {
            if (_cells[row, i] == number ||
                _cells[i, col] == number)
            {
                return true;
            }
        }

        int boxRow = row / 3 * 3;
        int boxCol = col / 3 * 3;

        for (int r = boxRow; r < boxRow + 3; r++)
        {
            for (int c = boxCol; c < boxCol + 3; c++)
            {
                if (_cells[r, c] == number)
                    return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Validates that the provided 9x9 grid obeys Sudoku constraints:
    /// no duplicate non-zero numbers in any row, column, or 3x3 box.
    /// </summary>
    private static bool IsValidGrid(int[,] grid)
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                int number = grid[row, col];

                if (number == 0)
                    continue;

                for (int i = 0; i < Size; i++)
                {
                    if (i != col && grid[row, i] == number)
                        return false;

                    if (i != row && grid[i, col] == number)
                        return false;
                }

                int boxRow = row / 3 * 3;
                int boxCol = col / 3 * 3;

                for (int r = boxRow; r < boxRow + 3; r++)
                {
                    for (int c = boxCol; c < boxCol + 3; c++)
                    {
                        if ((r != row || c != col) &&
                            grid[r, c] == number)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
    /// <summary>
    /// Throws if the given zero-based coordinates are outside the 9x9 grid.
    /// </summary>
    private static void CheckCoordinates(int row, int col)
    {
        if (row is < 0 or >= Size ||
            col is < 0 or >= Size)
        {
            throw new ArgumentOutOfRangeException(
                nameof(row), "Coordinates must be within the grid.");
        }
    }
}
