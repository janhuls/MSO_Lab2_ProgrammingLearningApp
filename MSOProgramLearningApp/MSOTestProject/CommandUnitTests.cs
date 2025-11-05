using MSOProgramLearningApp;

namespace MSOTestProject;

public class CommandUnitTests
{
    [Fact]
    public void TestMoveAddsToCharacterMoves()
    {
        var c = new Character();
        var m = new Move(3);

        m.Execute(c);

        Assert.Equal("Move 3", c.Moves[0]);
    }

    [Fact]
    public void TestTurnAddsToCharacterMoves()
    {
        var c = new Character();
        var t = new Turn(Side.Left);

        t.Execute(c);

        Assert.Equal("Turn left", c.Moves[0]);
    }

    [Fact]
    public void TestRepeatAddsInnerCommandsToCharacterMoves()
    {
        var c = new Character();
        var inner = new List<ICommand> { new Move(1), new Turn(Side.Right) };
        var r = new Repeat(2, inner);
        List<string> expectedResult = ["Move 1", "Turn right", "Move 1", "Turn right"];

        r.Execute(c);

        Assert.Equal(expectedResult, c.Moves);
    }

    [Fact]
    public void TestWallAheadConditionalRepeatAddsRightMoveCount()
    {
        var c = new Character();
        var inner = new List<ICommand> { new Move(1) };
        var cr = new ConditionalRepeat(inner, new WallAhead());

        cr.Execute(c);

        Assert.Equal(9, c.Moves.Count);
    }

    [Fact]
    public void TestWallAheadConditionalRepeatAddsRightMoves()
    {
        var c = new Character();
        var inner = new List<ICommand> { new Move(1) };
        var cr = new ConditionalRepeat(inner, new WallAhead());

        cr.Execute(c);

        Assert.All(c.Moves, cmd => Assert.Equal("Move 1", cmd));
    }

    [Fact]
    public void TestWallAheadConditionalRepeatRightEndPosition()
    {
        var c = new Character();
        var inner = new List<ICommand> { new Move(1) };
        var cr = new ConditionalRepeat(inner, new WallAhead());

        cr.Execute(c);

        Assert.Equal("(9, 0) facing east.", c.ToString());
    }
}