using MSOProgramLearningApp;

namespace MSOTestProject;

public class StringParserTest
{
    [Fact]
    public void TestParse()
    {
        List<ICommand> expectedCommands = new List<ICommand>();
        expectedCommands.Add(new Move(5));
        expectedCommands.Add(new Turn(Side.Right));
        List<ICommand> repeatCommands = new List<ICommand>();
        repeatCommands.Add(new Move(1));
        expectedCommands.Add(new Repeat(3, repeatCommands));
        string program = "Move 5\nTurn right\nRepeat 3\n    Move 1";
        
        StringParser s = new StringParser(program);
        List<ICommand> actualCommands = s.Parse();
        
        Assert.Equal(expectedCommands.Count, actualCommands.Count);
        for (int i = 0; i < expectedCommands.Count; i++)
        {
            Assert.Equal(expectedCommands[i].ToString(), actualCommands[i].ToString());
        }
    }

    [Fact]
    public void TestParserUtilsCountLeadingSpaces()
    {
        string testLine = "     test with 5 leading spaces";
        Assert.Equal(5, ParserUtils.CountLeadingSpaces(testLine));
    }
    
    
}