namespace Sudoku;

public sealed class ConsoleGame
{
    private readonly SudokuGame _game;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleGame(
        SudokuGame game,
        TextReader input,
        TextWriter output)
    {
        _game = game;
        _input = input;
        _output = output;
    }

    public void Run()
    {
        _output.WriteLine("=== SUDOKU ===");
        _output.WriteLine("Enter B3 7 to place a number.");
        _output.WriteLine("Enter C5 clear to clear a cell.");
        _output.WriteLine("Commands: hint, check, quit");

        while (true)
        {
            PrintBoard();

            if (_game.IsComplete())
            {
                _output.WriteLine("Congratulations! Puzzle solved!");
                return;
            }

            _output.Write("\n> ");
            string? input = _input.ReadLine();

            if (input is null)
                return;

            string[] parts = input.Split(
                ' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            string command = parts[0].ToLowerInvariant();

            if (command == "quit")
            {
                _output.WriteLine("Goodbye!");
                return;
            }

            if (command == "hint" && parts.Length == 1)
            {
                _game.TryHint(out string message);
                _output.WriteLine(message);
                continue;
            }

            if (command == "check" && parts.Length == 1)
            {
                _output.WriteLine(
                    _game.IsValidGrid()
                        ? "Grid is valid."
                        : "Grid contains conflicts.");
                continue;
            }

            if (!TryParseCell(parts[0], out int row, out int col))
            {
                _output.WriteLine(
                    "Invalid cell. Use a coordinate such as B3.");
                continue;
            }

            if (parts.Length == 2 &&
                parts[1].Equals(
                    "clear", StringComparison.OrdinalIgnoreCase))
            {
                _game.TryClear(row, col, out string message);
                _output.WriteLine(message);
                continue;
            }

            if (parts.Length == 2 &&
                int.TryParse(parts[1], out int number))
            {
                _game.TrySet(row, col, number, out string message);
                _output.WriteLine(message);
                continue;
            }

            _output.WriteLine(
                "Invalid command. Examples: B3 7, C5 clear, hint, check, quit.");
        }
    }

    private void PrintBoard()
    {
        _output.WriteLine();
        _output.WriteLine("    1 2 3   4 5 6   7 8 9");
        _output.WriteLine("  +-------+-------+-------+");

        for (int row = 0; row < 9; row++)
        {
            _output.Write($"{(char)('A' + row)} |");

            for (int col = 0; col < 9; col++)
            {
                int value = _game.GetCell(row, col);
                _output.Write(value == 0 ? " _" : $" {value}");

                if (col % 3 == 2)
                    _output.Write(" |");
            }

            _output.WriteLine();

            if (row % 3 == 2)
                _output.WriteLine("  +-------+-------+-------+");
        }
    }

    private static bool TryParseCell(
        string text, out int row, out int col)
    {
        row = -1;
        col = -1;

        if (text.Length != 2)
            return false;

        char r = char.ToUpperInvariant(text[0]);
        char c = text[1];

        if (r < 'A' || r > 'I' || c < '1' || c > '9')
            return false;

        row = r - 'A';
        col = c - '1';
        return true;
    }
}
