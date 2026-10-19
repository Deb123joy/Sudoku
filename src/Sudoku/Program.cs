namespace Sudoku;

internal static class Program
{
    private static void Main()
    {
        try
        {
            var generator = new SudokuGenerator();
            var (puzzle, solution) = generator.Generate();

            var game = new SudokuGame(puzzle, solution);
            var console = new ConsoleGame(game, Console.In, Console.Out);

            console.Run();
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"Unable to start game: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }
}
