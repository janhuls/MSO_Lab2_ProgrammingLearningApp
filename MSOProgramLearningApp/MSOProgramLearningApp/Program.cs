namespace MSOProgramLearningApp;

internal static class Program
{
    //CLI
    private static void Main()
    {
        Console.WriteLine("=== Programming Learning App ===");
        Console.WriteLine("1. Load example program");
        Console.WriteLine("2. Load from file");
        Console.Write("Choose option: ");

        var choice = int.Parse(Console.ReadLine() ?? "1");
        List<ICommand> commands;
        string name;

        if (choice == 1)
        {
            Console.WriteLine("Choose example: 1) Basic 2) Advanced 3) Expert");
            var exampleChoice = int.Parse(Console.ReadLine() ?? "1");
            (name, commands) = ExamplePrograms.GetExample(exampleChoice);
        }
        else
        {
            Console.Write("Enter file name: ");
            var fileName = Console.ReadLine() ?? "input.txt";
            var input = File.ReadAllText(fileName);
            var parser = new StringParser(input);

            commands = parser.Parse();
            name = fileName;
        }

        Console.WriteLine("1. Execute Program\n2. Show Metrics");
        var action = int.Parse(Console.ReadLine() ?? "1");

        Character c = new(Grid.XSquareFalse(10));
        if (action == 1)
        {
            Console.WriteLine($"\nExecuting {name}:");

            foreach (var cmd in commands) cmd.Execute(c);
            Console.WriteLine(string.Join(", ", c.Moves) + ".");
            Console.WriteLine($"End state {c}");
        }
        else
        {
            var metrics = new MetricsCalculator(new BasicMetricsStrategy());
            Console.WriteLine(metrics.Calculate(commands));
        }
    }
}