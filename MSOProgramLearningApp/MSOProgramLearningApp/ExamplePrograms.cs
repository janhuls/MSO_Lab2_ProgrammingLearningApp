namespace MSOProgramLearningApp;

//simple builder for making hardcoded example programs
public class ProgramBuilder
{
    private readonly List<ICommand> _commands = new();

    public ProgramBuilder Move(int steps)
    {
        _commands.Add(new Move(steps));
        return this;
    }

    public ProgramBuilder Turn(Side side)
    {
        _commands.Add(new Turn(side));
        return this;
    }

    public ProgramBuilder Repeat(int times, Action<ProgramBuilder> block)
    {
        var innerBuilder = new ProgramBuilder();
        block(innerBuilder);
        _commands.Add(new Repeat(times, innerBuilder.Build()));
        return this;
    }

    public List<ICommand> Build() => _commands;
}

//stores example programs for easy use
public static class ExamplePrograms
{
    public static (string name, List<ICommand> program) GetExample(int level)
    {
        return level switch
        {
            1 => ("Basic",
                new ProgramBuilder().Move(10)
                    .Turn(Side.Right)
                    .Move(10)
                    .Turn(Side.Right)
                    .Move(10)
                    .Turn(Side.Right)
                    .Move(10)
                    .Turn(Side.Right)
                    .Build()),
            2 => ("Advanced", new ProgramBuilder().Repeat(4, b => b.Move(10).Turn(Side.Right)).Build()),
            3 => ("Expert",
                new ProgramBuilder().Move(5)
                    .Turn(Side.Left)
                    .Turn(Side.Left)
                    .Move(3)
                    .Turn(Side.Right)
                    .Repeat(3, r => r.Move(1).Turn(Side.Right).Repeat(5, s => s.Move(2)))
                    .Turn(Side.Left)
                    .Build()),
            _ => throw new ArgumentException("Invalid example choice.")
        };
    }
}