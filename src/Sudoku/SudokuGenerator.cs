namespace Sudoku;

public sealed class SudokuGenerator
{
    private readonly Random _random;

    public SudokuGenerator(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public (int[,] Puzzle, int[,] Solution) Generate()
    {
        var solution = new int[9, 9];

        if (!Fill(solution))
            throw new InvalidOperationException(
                "Unable to generate a Sudoku solution.");

        var puzzle = (int[,])solution.Clone();

        var positions = Enumerable.Range(0, 81)
            .OrderBy(_ => _random.Next())
            .ToArray();

        int clues = 81;

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
                    .ToArray();

                foreach (int number in numbers)
                {
                    if (!CanPlace(board, row, col, number))
                        continue;

                    board[row, col] = number;

                    if (Fill(board))
                        return true;

                    board[row, col] = 0;
                }

                return false;
            }
        }

        return true;
    }

    private static bool CanPlace(
        int[,] board, int row, int col, int number)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == number ||
                board[i, col] == number)
                return false;
        }

        int startRow = row / 3 * 3;
        int startCol = col / 3 * 3;

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
