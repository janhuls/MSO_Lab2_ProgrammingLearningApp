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

public abstract class Repeatable : ICommand
{
    public abstract void Execute(Character c);
    protected List<ICommand> commands;
    public List<ICommand> GetCommands() => commands;
}

public class Repeat : Repeatable
{
    private readonly int _repetitions;

    public Repeat(int repetitions, List<ICommand> commands)
    {
        _repetitions = repetitions;
        this.commands = commands;
    }

    public override void Execute(Character c)
    {
        for (int i = 0; i < _repetitions; i++)
            foreach (var command in commands)
                command.Execute(c);
    }
}

public class ConditionalRepeat : Repeatable
{
    private readonly ICondition _condition;

    public ConditionalRepeat(List<ICommand> commands, ICondition condition)
    {
        _condition = condition;
        this.commands = commands;
    }

    public override void Execute(Character c)
    {
        while (!_condition.Evaluate(c))
            foreach (var t in commands)
            {
                t.Execute(c);
                if (_condition.Evaluate(c))
                    return;
            }
    }
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
        return x >= c.Grid.GetSize() || y >= c.Grid.GetSize() || x < 0 || y < 0;
    }
}