namespace ProgrammingLearningApp;

public interface IMetricsStrategy
{
    void Calculate(List<ICommand> commands);
}

public class BasicMetricsStrategy : IMetricsStrategy
{
    public void Calculate(List<ICommand> commands)
    {
        int totalCommands = 0, repeatCount = 0, maxDepth = 0;
        Analyze(commands, 0, ref totalCommands, ref repeatCount, ref maxDepth);

        Console.WriteLine($"Commands: {totalCommands}");
        Console.WriteLine($"Repeat Commands: {repeatCount}");
        Console.WriteLine($"Max Nesting Depth: {maxDepth}");
    }

    private void Analyze(List<ICommand> commands, int depth, ref int total, ref int repeats, ref int maxDepth)
    {
        foreach (var cmd in commands)
        {
            total++;
            if (cmd is Repeat r)
            {
                repeats++;
                maxDepth = Math.Max(maxDepth, depth + 1);
                Analyze(r.GetCommands(), depth + 1, ref total, ref repeats, ref maxDepth);
            }
        }
    }
}