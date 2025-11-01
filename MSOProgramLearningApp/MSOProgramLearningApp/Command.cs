namespace MSOProgramLearningApp;

public interface ICommand
{
    //changes the character c based on the command
    void Execute(Character c);
}

public class Turn(Side side) : ICommand
{
    //rotates c to the side specified
    public void Execute(Character c)
    {
        c.Rotate(side);
        c.Moves.Add($"Turn {side.ToString().ToLower()}");
    }
}

public class Move(int amount) : ICommand
{
    //moves c by the amount specified
    public void Execute(Character c)
    {
        c.Move(amount);
        c.Moves.Add($"Move {amount}");
    }
}

public abstract class Repeatable : ICommand
{
    //changes the character based on the repeatable implementation
    public abstract void Execute(Character c);
    //stores the commands in the repeatable
    protected List<ICommand> Commands = new();
    //returns the commands in the repeatable
    public List<ICommand> GetCommands() => Commands;
}

public class Repeat : Repeatable
{
    //times the repeat will execute the command blocks
    private readonly int _repetitions;
    
    public Repeat(int repetitions, List<ICommand> commands)
    {
        _repetitions = repetitions;
        Commands = commands;
    }
    //repeats the block in Commands for _repetitions times
    public override void Execute(Character c)
    {
        for (int i = 0; i < _repetitions; i++)
            foreach (var command in Commands)
                command.Execute(c);
    }
}

public class ConditionalRepeat : Repeatable
{
    //stores the condition 
    private readonly ICondition _condition;

    public ConditionalRepeat(List<ICommand> commands, ICondition condition)
    {
        _condition = condition;
        Commands = commands;
    }
    
    //repeats the block in Commands until the condition is true
    public override void Execute(Character c)
    {
        while (!_condition.Evaluate(c))
            foreach (var t in Commands)
            {
                t.Execute(c);
                if (_condition.Evaluate(c))
                    return;
            }
    }
}

public interface ICondition
{
    //gets a bool to see if a condition is true on a character c
    public bool Evaluate(Character c);
}

public class WallAhead : ICondition
{
    //evaluates whether the space ahead of c is a wall
    public bool Evaluate(Character c)
    {
        var (x, y) = c.CalcMove(1);
        return c.Grid.IsWall(x, y);
    }
}

public class GridEdge : ICondition
{
    //evaluates whether the space ahead of c is outside the grid
    public bool Evaluate(Character c)
    {
        var (x, y) = c.CalcMove(1);
        return c.Grid.OutOfBounds(x, y);
    }
}