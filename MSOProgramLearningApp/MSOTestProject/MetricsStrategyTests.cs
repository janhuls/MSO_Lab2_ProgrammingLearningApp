using MSOProgramLearningApp;

namespace MSOTestProject;

public class MetricsStrategyTests
{
    [Fact]
    public void TestBasicStrategy()
    {
        var strategy = new BasicMetricsStrategy();
        var commands = new List<ICommand>
        {
            new Move(5),
            new Turn(Side.Right)
        };
        var repeatCommands = new List<ICommand> { new Move(1) };
        commands.Add(new Repeat(3, repeatCommands));

        strategy.Calculate(commands);
        const string expectedResult = "Commands: 4\nRepeat Commands: 1\nMax Nesting Depth: 1\n";
        Assert.Equal(expectedResult, strategy.GetResult());
    }
}