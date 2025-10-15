namespace ProgrammingLearningApp.ProgrammingLearningApp;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Programming Learning App ===");
        Console.WriteLine("1. Load example program");
        Console.WriteLine("2. Load from file");
        Console.Write("Choose option: ");

        int choice = int.Parse(Console.ReadLine() ?? "1");
        IParser parser;
        List<ICommand> commands;
        string name;

        if (choice == 1)
        {
            Console.WriteLine("Choose example: 1) Basic 2) Advanced 3) Expert");
            int exampleChoice = int.Parse(Console.ReadLine() ?? "1");
            (name, commands) = ExamplePrograms.GetExample(exampleChoice);
        }
        else
        {
            Console.Write("Enter file name: ");
            string fileName = Console.ReadLine() ?? "input.txt";
            string input = System.IO.File.ReadAllText(fileName);
            parser = new StringParser(input);

            commands = parser.Parse();
            name = fileName;
        }

        Console.WriteLine("1. Execute Program\n2. Show Metrics");
        int action = int.Parse(Console.ReadLine() ?? "1");

        Character c = new();
        if (action == 1)
        {
            Console.WriteLine($"\nExecuting {name}:");

            List<string> cmds = new();
            foreach (var cmd in commands)
            {
                cmd.Execute(c);
                cmds.Add(cmd.ToString());
            }
            Console.WriteLine(String.Join(", ", cmds) + ".");
            Console.WriteLine($"End state {c}");
        }
        else
        {
            var metrics = new MetricsCalculator(new BasicMetricsStrategy());
            metrics.Calculate(commands);
        }
    }
}