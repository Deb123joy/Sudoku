using Sudoku;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            SudokuGenerator generator = new SudokuGenerator();// Create a new sudoku puzzle generator
            // Generate a puzzle and its solution
            var generated = generator.Generate();

            int[,] puzzle = generated.Puzzle;
            int[,] solution = generated.Solution;

            SudokuGame game = new SudokuGame(puzzle, solution);// Create the game instance with the generated puzzle and solution
            ConsoleGame console = new ConsoleGame(game, Console.In, Console.Out);// Create the console UI with standard input/output

            console.Run();
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"Unable to start game: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }
}