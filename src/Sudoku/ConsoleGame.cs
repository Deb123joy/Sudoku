namespace Sudoku;

public sealed class ConsoleGame
{
    private readonly SudokuGame _game;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    /// <summary>
    /// Creates a console UI binder for a <see cref="SudokuGame"/>.
    /// </summary>
    public ConsoleGame(
        SudokuGame game,
        TextReader input,
        TextWriter output)
    {
        _game = game;
        _input = input;
        _output = output;
    }
    /// <summary>
    /// Main interactive loop. Displays the board, reads commands from the
    /// input, dispatches actions (set, clear, hint, check, quit) to the game,
    /// and writes responses to the output.
    /// </summary>
    public void Run()
    {
        _output.WriteLine("~~~");
        _output.WriteLine("Welcome to Sudoku!");
        _output.WriteLine("Here is your puzzle:"); 
     

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

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

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
                        ? "No rule violations detected."
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
               bool MoveAccepted=  _game.TrySet(row, col, number, out string message);
                
                _output.WriteLine(message);
                if(MoveAccepted)
                _output.WriteLine("Current grid:");
                continue;
            }

            _output.WriteLine(
                "Invalid command. Examples: B3 7, C5 clear, hint, check, quit.");
        }
    }
    /// <summary>
    /// Renders the current puzzle board to the output in a readable
    /// grid with row labels ('A'..'I') and column labels (1..9).
    /// </summary>
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
        _output.WriteLine("Enter command(e.g., A3 4, C5 clear, hint, check):");
    }
    /// <summary>
    /// Parses a two-character cell coordinate (letter A-I followed by digit 1-9).
    /// Converts the human coordinate into zero-based row and column indices.
    /// Returns true when parsing and validation succeed; otherwise false.
    /// </summary>
    private static bool TryParseCell(
        string text, out int row, out int col)
    {
        // Initialize out parameters with invalid defaults.
        row = -1;
        col = -1;
        // Expect exactly two characters: a letter (A-I) and a digit (1-9).
        // Examples of valid inputs: "A1", "B3", "I9".
        if (text.Length != 2)
            return false;
        // Normalize the row character to uppercase so both 'a' and 'A' work.
        char r = char.ToUpperInvariant(text[0]);
        char c = text[1];
        // Validate that row is within 'A'..'I' and column within '1'..'9'.
        if (r < 'A' || r > 'I' || c < '1' || c > '9')
            return false;
        //Convert the coordinate into array indexes
        //Ex: For "B3":  row = 'B' - 'A'; // 1, col = '3' - '1'; // 2
        // Convert characters to zero-based indices:
        // 'A' -> 0, 'B' -> 1, ..., 'I' -> 8
        // '1' -> 0, '2' -> 1, ..., '9' -> 8
        row = r - 'A';
        col = c - '1';
        return true;
    }
}
