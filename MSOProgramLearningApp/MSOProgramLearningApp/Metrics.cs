namespace MSOProgramLearningApp;
public interface IMetricsStrategy
{
    public string Calculate(List<ICommand> commands);
}

public class BasicMetricsStrategy : IMetricsStrategy
{
    private string _result = "";
    public string Calculate(List<ICommand> commands)
    {
        int totalCommands = 0, repeatCount = 0, maxDepth = 0;
        Analyze(commands, 0, ref totalCommands, ref repeatCount, ref maxDepth);
        
        _result = $"Commands: {totalCommands}\n" +
                  $"Repeat Commands: {repeatCount}\n" +
                  $"Max Nesting Depth: {maxDepth}\n";
        return _result;
    }

    private static void Analyze(List<ICommand> commands, int depth, ref int total, ref int repeats, ref int maxDepth)
    {
        if (commands == null) return;
        
        foreach (var cmd in commands)
        {
            total++;
            if (cmd is Repeatable r)
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
        return _result;
    }
}

public class MetricsCalculator(IMetricsStrategy strategy)
{
    public string Calculate(List<ICommand> commands) => strategy.Calculate(commands);
}
