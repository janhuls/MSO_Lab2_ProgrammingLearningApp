namespace MSOTestProject;

using MSOProgramLearningApp;

public class CommandUnitTests
{
    [Fact]
    public void TestMoveAddsToCharacterMoves()
    {
        Character c = new Character();
        Move m = new Move(3);

        m.Execute(c);

        Assert.Equal(m.ToString(), c.Moves[0]);
    }

    [Fact]
    public void TestTurnAddsToCharacterMoves()
    {
        Character c = new Character();
        Turn t = new Turn(Side.Left);

        t.Execute(c);

        Assert.Equal(t.ToString(), c.Moves[0]);
    }

    [Fact]
    public void TestRepeatAddsInnerCommandsToCharacterMoves()
    {
        Character c = new Character();
        var inner = new List<ICommand> { new Move(1), new Turn(Side.Right) };
        Repeat r = new Repeat(2, inner);
        List<string> expectedResult = ["Move 1", "Turn right", "Move 1", "Turn right"];
        
        r.Execute(c);
        
        Assert.Equal(expectedResult, c.Moves);
    }

    [Fact]
    public void TestWallAheadConditionalRepeatAddsRightMoveCount()
    {
        Character c = new Character();
        var inner = new List<ICommand> { new Move(1) };
        ConditionalRepeat cr = new ConditionalRepeat(inner, new WallAhead());

        cr.Execute(c);

        Assert.Equal(9, c.Moves.Count);
    }

    [Fact]
    public void TestWallAheadConditionalRepeatAddsRightMoves()
    {
        Character c = new Character();
        var inner = new List<ICommand> { new Move(1) };
        ConditionalRepeat cr = new ConditionalRepeat(inner, new WallAhead());

        cr.Execute(c);
        
        Assert.All(c.Moves, cmd => Assert.Equal("Move 1", cmd.ToString()));
    }

    [Fact]
    public void TestWallAheadConditionalRepeat()
    {
        Character c = new Character();
        List<ICommand> inner = new List<ICommand> { new Move(1) };
        ConditionalRepeat cr = new ConditionalRepeat(inner, new WallAhead());

        cr.Execute(c);

        Assert.Equal("(9, 0) facing east.", c.ToString());
    }
}