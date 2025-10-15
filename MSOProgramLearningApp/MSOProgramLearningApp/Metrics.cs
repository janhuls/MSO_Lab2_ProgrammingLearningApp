namespace MSOProgramLearningApp;
public interface IMetricsStrategy
{
    void Calculate(List<ICommand> commands);
}

public class BasicMetricsStrategy : IMetricsStrategy
{
    private string result = "";
    public void Calculate(List<ICommand> commands)
    {
        int totalCommands = 0, repeatCount = 0, maxDepth = 0;
        Analyze(commands, 0, ref totalCommands, ref repeatCount, ref maxDepth);
        
        // save it so that it can be tested
        result = $"Commands: {totalCommands}\n" +
               $"Repeat Commands: {repeatCount}\n" +
               $"Max Nesting Depth: {maxDepth}\n";
        Console.Write(result);
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

    // for testing purposes
    public string GetResult()
    {
        return result;
    }
}

public class MetricsCalculator
{
    private readonly IMetricsStrategy strategy;

    public MetricsCalculator(IMetricsStrategy strategy)
    {
        this.strategy = strategy;
    }

    public void Calculate(List<ICommand> commands) => strategy.Calculate(commands);
}
