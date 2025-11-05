using MSOProgramLearningApp;

namespace MSOTestProject;

public class StringParserTest
{
    [Fact]
    public void TestParse()
    {
        var expectedCommands = new List<ICommand>
        {
            new Move(5),
            new Turn(Side.Right)
        };
        var repeatCommands = new List<ICommand> { new Move(1) };
        expectedCommands.Add(new Repeat(3, repeatCommands));
        const string program = "Move 5\nTurn right\nRepeat 3\n    Move 1";

        var s = new StringParser(program);
        var actualCommands = s.Parse();

        Assert.Equal(expectedCommands.Count, actualCommands.Count);
        for (var i = 0; i < expectedCommands.Count; i++)
            Assert.Equal(expectedCommands[i].ToString(), actualCommands[i].ToString());
    }

    [Fact]
    public void TestParserUtilsCountLeadingSpaces()
    {
        const string testLine = "     test with 5 leading spaces";
        Assert.Equal(5, ParserUtils.CountLeadingSpaces(testLine));
    }
}