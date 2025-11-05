using MSOProgramLearningApp;

namespace MSOTestProject;

public class CharacterUnitTests
{
    [Fact]
    public void TestMove()
    {
        var c = new Character();
        c.Move(5);
        Assert.Equal("(5, 0) facing east.", c.ToString());
    }

    [Fact]
    public void TestRotateL()
    {
        var c = new Character();
        c.Rotate(Side.Left);
        Assert.Equal("(0, 0) facing north.", c.ToString());
    }

    [Fact]
    public void TestRotateR()
    {
        var c = new Character();
        c.Rotate(Side.Right);
        Assert.Equal("(0, 0) facing south.", c.ToString());
    }

    [Fact]
    public void TestRotateFullCircle()
    {
        var c = new Character();
        c.Rotate(Side.Right);
        c.Rotate(Side.Right);
        c.Rotate(Side.Right);
        c.Rotate(Side.Right);
        Assert.Equal("(0, 0) facing east.", c.ToString());
    }

    [Fact]
    public void TestComplexMovementSequence()
    {
        var c = new Character();
        c.Move(3);
        c.Rotate(Side.Right);
        c.Move(2);
        c.Rotate(Side.Left);
        c.Move(1);
        Assert.Equal("(4, 2) facing east.", c.ToString());
    }
}