namespace ProgrammingLearningApp;

public class ProgramParser
{
    private const int IndentSize = 4;
    public ProgramParser() { }
    public List<ICommand> Parse(string input)
    {
        var lines = input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Replace("\r", ""))
            .ToArray();
        int index = 0;
        return ParseBlock(lines, ref index, 0);
    }

    private List<ICommand> ParseBlock(string[] lines, ref int index, int expectedIndent)
    {
        var commands = new List<ICommand>();

        while (index < lines.Length)
        {
            string line = lines[index];
            int indent = ParserUtils.CountLeadingSpaces(line);

            if (indent < expectedIndent)
                break;

            if (indent > expectedIndent)
                throw new Exception($"Unexpected indentation at line {index + 1}: '{line}'");

            string trimmed = line.Trim();

            if (trimmed.StartsWith("Repeat", StringComparison.OrdinalIgnoreCase))
            {
                commands.Add(ParseRepeat(lines, ref index, expectedIndent));
            }
            else
            {
                commands.Add(ParseSimpleCommand(trimmed));
                index++;
            }
        }
        return commands;
    }
    
    private ICommand ParseRepeat(string[] lines, ref int index, int expectedIndent)
    {
        string line = lines[index].Trim();

        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (!int.TryParse(parts[1], out int times))
            throw new Exception($"Invalid repeat syntax at line {index + 1}: {line}");

        index++;

        var innerCommands = ParseBlock(lines, ref index, expectedIndent + 4);
        return new Repeat(times, innerCommands);
    }

    private ICommand ParseSimpleCommand(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return CreateCommand(parts);
    }
    private ICommand CreateCommand(string[] parts)
    {
        return parts[0] switch
        {
            "Move" => new Move(int.Parse(parts[1])),
            "Turn" => new Turn((SIDE)Enum.Parse(typeof(SIDE), parts[1], true)),
            _ => throw new Exception($"Unknown command: {parts[0]}")
        };
    }
}


public static class ParserUtils
{
    public static int CountLeadingSpaces(string line)
    {
        int count = 0;
        foreach (char c in line)
        {
            if (c == ' ') count++;
            else break;
        }
        return count;
    }
}

public class ProgramBuilder
{
    private readonly List<ICommand> commands = new();

    public ProgramBuilder Move(int steps)
    {
        commands.Add(new Move(steps));
        return this;
    }

    public ProgramBuilder Turn(SIDE side)
    {
        commands.Add(new Turn(side));
        return this;
    }

    public ProgramBuilder Repeat(int times, Action<ProgramBuilder> block)
    {
        var innerBuilder = new ProgramBuilder();
        block(innerBuilder);
        commands.Add(new Repeat(times, innerBuilder.Build()));
        return this;
    }

    public List<ICommand> Build() => commands;
}

public static class ExamplePrograms
{
    public static (string name, List<ICommand> program) GetExample(int level)
    {
        switch (level)
        {
            case 1:
                return 
                ("Basic", new ProgramBuilder()
                .Move(10).Turn(SIDE.RIGHT)
                .Move(10).Turn(SIDE.RIGHT)
                .Move(10).Turn(SIDE.RIGHT)
                .Move(10).Turn(SIDE.RIGHT)
                .Build());
            case 2:
                return
                ("Advanced", new ProgramBuilder()
                .Repeat(4, b => b.Move(10).Turn(SIDE.RIGHT))
                .Build());
            case 3:
                return
                ("Expert", new ProgramBuilder()
                .Move(5).Turn(SIDE.LEFT).Turn(SIDE.LEFT).Move(3)
                .Turn(SIDE.RIGHT)
                .Repeat(3, r => r.Move(1).Turn(SIDE.RIGHT)
                    .Repeat(5, s => s.Move(2)))
                .Turn(SIDE.LEFT)
                .Build());
            default:
                throw new ArgumentException("Invalid example choice.");
        };
    }
}
public enum DIRECTION
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}
public enum SIDE
{
    LEFT,
    RIGHT
}

public class Character 
{
    private int posX { get; set; }
    private int posY { get; set; }

    private DIRECTION rotation { get; set; }

    public Character()
    {
        posX = 0;
        posY = 0;
        rotation = DIRECTION.EAST;
    }

    public void Move(int amount)
    {
        switch (rotation)
        {
            case DIRECTION.NORTH:
                posY += amount;
                break;
            case DIRECTION.EAST:
                posX += amount;
                break;
            case DIRECTION.SOUTH:
                posY -= amount;
                break;
            case DIRECTION.WEST:
                posX -= amount;
                break;
        }
    }
    
    public void Rotate(SIDE side)
    {
        int rot = (int)this.rotation;
        switch (side)
        {
            case SIDE.LEFT:
                rotation = (DIRECTION)((rot + 3) % 4);
                break;
            case SIDE.RIGHT:
                rotation = (DIRECTION)((rot + 1) % 4);
                break;
        }
    }

    public override string ToString()
    {
        return $"({posX}, {posY}) facing {rotation.ToString().ToLower()}.";
    }
}