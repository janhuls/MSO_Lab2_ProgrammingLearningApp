using System.ComponentModel;
using System.Data;

namespace MSOProgramLearningApp;

public interface ICommand
{
    void Execute(Character c);
}

public class Turn(Side side) : ICommand
{
    public void Execute(Character c)
    {
        c.Rotate(side);
        c.Moves.Add($"Turn {side.ToString().ToLower()}");
    }
}

public class Move(int amount) : ICommand
{
    public void Execute(Character c)
    {
        c.Move(amount);
        c.Moves.Add($"Move {amount}");
    }
}

public class Repeat(int repetitions, List<ICommand> commands) : ICommand
{
    public void Execute(Character c)
    {
        for (int i = 0; i < repetitions; i++)
            foreach (var command in commands)
                command.Execute(c);
    }
    public List<ICommand> GetCommands() => commands;
}

public class ConditionalRepeat(List<ICommand> commands, ICondition condition) : ICommand
{
    public void Execute(Character c)
    {
        while (condition.Evaluate(c))
            foreach (var com in commands)
                com.Execute(c);
    }
    public List<ICommand> GetCommands() => commands;
}

public interface ICondition
{
    public bool Evaluate(Character c);
}

public class WallAhead : ICondition
{
    public bool Evaluate(Character c)
    {
        var (x, y) = c.CalcMove(1);
        return c.Grid.IsWall(x, y);
    }
}

public class GridEdge : ICondition
{
    public bool Evaluate(Character c)
    {
        var (x, y) = c.CalcMove(1);
        return x >= c.Grid.GetWidth() || y >= c.Grid.GetHeight();
    }
}