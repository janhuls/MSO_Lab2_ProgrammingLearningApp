namespace MSOTestProject;

using MSOProgramLearningApp;

public class CommandUnitTests
{
    [Fact]
    public void TestMoveToString()
    {
        Move m = new Move(5);
        Assert.Equal("Move 5", m.ToString());
    }
    
    [Fact]
    public void TestTurnToString()
    {
        Turn t = new Turn(Side.Right);
        Assert.Equal("Turn right", t.ToString());
    }

    [Fact]
    public void TestRepeatToString()
    {
        List<ICommand> inner = new List<ICommand> { new Move(1) };
        Repeat r = new Repeat(3, inner);
        // Expect a line for the Repeat and indented inner command(s) with 4 spaces
        Assert.Equal("Repeat 3\n    Move 1", r.ToString());
    }
}