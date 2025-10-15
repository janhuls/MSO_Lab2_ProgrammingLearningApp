namespace MSOProgramLearningApp;

public interface ICommand
{
    void Execute(Character c);
    string ToString();
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
    public List<ICommand> GetCommands() => commands;
    public override string ToString() 
    {
        List<string> ss = new();
        for (int i = 0; i < repetitions; i++)
        {
            foreach (var cmd in commands)
                ss.Add(cmd.ToString());
        }
        return String.Join(", ", ss);
    }
} 