namespace Sudoku;

public sealed class SudokuGenerator
{
    private readonly Random _random;
    /// Creates a new generator. An optional <paramref name="random"/>
    public SudokuGenerator(Random? random = null)
    {
        _random = random ?? new Random();
    }
    /// <summary>
    /// Generates a complete Sudoku solution and then removes numbers
    /// to produce a puzzle with a unique solution and exactly
    /// <see cref="SudokuGame.ClueCount"/> clues.
    /// Returns a tuple containing the puzzle (with empty cells as 0)
    /// and the full solution.
    /// </summary>
    public (int[,] Puzzle, int[,] Solution) Generate()
    {
        var solution = new int[9, 9];

        if (!Fill(solution))
            throw new InvalidOperationException(
                "Unable to generate a Sudoku solution.");

        var puzzle = (int[,])solution.Clone();

        var positions = Enumerable.Range(0, 81)
            .OrderBy(_ => _random.Next())
            .ToArray();//generate a random order of positions from 0 to 80 (representing the 81 cells in the Sudoku grid) and store them in an array called positions

        int clues = 81;
        // Iterate through the positions in random order, attempting to remove numbers for the user to play in the puzzle array while ensuring a unique solution remains in solution array.
        foreach (int position in positions)
        {
            if (clues <= SudokuGame.ClueCount)
                break;

            int row = position / 9;
            int col = position % 9;
            int previous = puzzle[row, col];

            puzzle[row, col] = 0;

            if (CountSolutions(puzzle, 2) == 1)
            {
                clues--;
            }
            else
            {
                puzzle[row, col] = previous;
            }
        }

        if (clues != SudokuGame.ClueCount)
        {
            throw new InvalidOperationException(
                "Could not generate a unique puzzle with exactly " +
                $"{SudokuGame.ClueCount} clues.");
        }

        return (puzzle, solution);
    }
    /// <summary>
    /// Recursively fills the provided board with a valid complete Sudoku
    /// solution using backtracking. Returns true when the board is fully filled.
    /// </summary>
    private bool Fill(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] != 0)
                    continue;

                int[] numbers = Enumerable.Range(1, 9)
                    .OrderBy(_ => _random.Next())
                    .ToArray();//generate a random order of numbers 1-9 and store them in an array called numbers

                foreach (int number in numbers)
                {
                    if (!CanPlace(board, row, col, number))
                        continue;

                    board[row, col] = number;

                    if (Fill(board))
                        return true;

                    board[row, col] = 0;
                }

                return false;//tried all possible numbers for this empty cell, and none worked
            }
        }

        return true;//There are no empty cells left, so the board is complete.
    }
    /// <summary>
    /// Determines whether <paramref name="number"/> can legally be placed
    /// at the given zero-based <paramref name="row"/> and <paramref name="col"/>
    /// without violating Sudoku row, column, or 3x3 box constraints.
    /// </summary>
    private static bool CanPlace(
        int[,] board, int row, int col, int number)
    {
        // Check the row and column for conflicts.
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == number ||
                board[i, col] == number)
                return false;
        }
        //Find the starting position of the 3×3 subgrid that contains the cell at (row, col).
        int startRow = row / 3 * 3;
        int startCol = col / 3 * 3;
        //Check all nine cells in the subgrid to see if any of them contain the number. If any cell does, return false.
        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                if (board[r, c] == number)
                    return false;
            }
        }

        return true;
    }
    /// <summary>
    /// Counts how many solutions the given partial <paramref name="board"/>
    /// has, up to the specified <paramref name="limit"/>. Uses a heuristic
    /// that selects the empty cell with the fewest candidates to speed up search.
    /// Returns a number in the range [0, limit].
    /// </summary>
    private static int CountSolutions(int[,] board, int limit)
    {
        int row = -1;
        int col = -1;
        int fewestCandidates = 10;

        // Choose the empty cell with the fewest legal values.
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (board[r, c] != 0)
                    continue;

                int candidates = 0;

                for (int n = 1; n <= 9; n++)
                {
                    if (CanPlace(board, r, c, n))
                        candidates++;
                }

                if (candidates < fewestCandidates)
                {
                    fewestCandidates = candidates;
                    row = r;
                    col = c;
                }

                if (candidates == 0)
                    return 0;
            }
        }
        // If no empty cells remain, the board is a valid complete solution.
        if (row == -1)
            return 1;

        int count = 0;

        for (int number = 1; number <= 9; number++)
        {
            if (!CanPlace(board, row, col, number))
                continue;

            board[row, col] = number;
            count += CountSolutions(board, limit - count);
            board[row, col] = 0;

            if (count >= limit)
                return count;
        }

        return count;
    }
}
