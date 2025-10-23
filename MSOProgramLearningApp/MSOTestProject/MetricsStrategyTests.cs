namespace MSOTestProject;
using MSOProgramLearningApp;
public class MetricsStrategyTests
{
    [Fact]
    public void TestBasicStrategy()
    {
        BasicMetricsStrategy strategy = new BasicMetricsStrategy();
        List<ICommand> commands = new List<ICommand>();
        commands.Add(new Move(5));
        commands.Add(new Turn(Side.Right));
        List<ICommand> repeatCommands = new List<ICommand>();
        repeatCommands.Add(new Move(1));
        commands.Add(new Repeat(3, repeatCommands));
        
        strategy.Calculate(commands);
        string expectedResult = "Commands: 4\nRepeat Commands: 1\nMax Nesting Depth: 1\n";
        Assert.Equal(expectedResult, strategy.GetResult());
    }
}