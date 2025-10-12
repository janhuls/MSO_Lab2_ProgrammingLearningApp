namespace ProgrammingLearningApp;

public interface ICommand
{
    void Execute(Character c);
}

public class Turn : ICommand
{
    private SIDE side;

    public Turn(SIDE side)
    {
        this.side = side;
    }
    public void Execute(Character c)
    {
        c.Rotate(side);
        Console.WriteLine(this.ToString());
    }
    public override string ToString()
    {
        return $"Turn {side.ToString().ToLower()}";
    }
}

public class Move : ICommand
{
    private int amount;

    public Move(int amount)
    {
        this.amount = amount;
    }
    public void Execute(Character c)
    {
        c.Move(amount);
        Console.WriteLine(this.ToString());
    }
    public override string ToString()
    {
        return $"Move {amount}";
    }
}

public class Repeat : ICommand
{
    private List<ICommand> commands;
    private int repetitions;

    public Repeat(int repetitions, List<ICommand> commands)
    {
        this.repetitions = repetitions;
        this.commands = commands;
    }
    public void Execute(Character c)
    {
        for (int i = 0; i < repetitions; i++)
        {
            foreach (var command in commands)
            {
                command.Execute(c);
            }
        }
    }
}