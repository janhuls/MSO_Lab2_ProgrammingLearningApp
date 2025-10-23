using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MSOProgramLearningApp;
public interface IParser
{
    public List<ICommand> Parse();
}

public class StringParser(string program) : IParser
{
    private const int IndentSize = 4;

    public List<ICommand> Parse()
    {
        var lines = program
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
                commands.Add(ParseRepeat(lines, ref index, expectedIndent));
            else
            {
                commands.Add(ParseSimpleCommand(trimmed));
                index++;
            }
        }
        return commands;
    }

    private Repeat ParseRepeat(string[] lines, ref int index, int expectedIndent)
    {
        string line = lines[index].Trim();

        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (!int.TryParse(parts[1], out int times))
            throw new Exception($"Invalid repeat syntax at line {index + 1}: {line}");

        index++;

        var innerCommands = ParseBlock(lines, ref index, expectedIndent + IndentSize);
        return new Repeat(times, innerCommands);
    }

    private static ICommand ParseSimpleCommand(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return CreateCommand(parts);
    }

    private static ICommand CreateCommand(string[] parts) =>
        parts[0] switch
        {
            "Move" => new Move(int.Parse(parts[1])),
            "Turn" => new Turn((Side)Enum.Parse(typeof(Side), parts[1], true)),
            _ => throw new Exception($"Unknown command: {parts[0]}")
        };
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
        ;
    }
}
public enum Direction
{
    North,
    East,
    South,
    West
}
public enum Side
{
    Left,
    Right
}

public class Grid
{
    private readonly bool[,] _walls; //True is a wall
    
    public static Grid TenSquareFalse()
    {
        var array = new bool[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                array[i, j] = false;
        return new Grid(array);
    }

    private Grid(bool[,] grid)
    {
        this._walls = grid;
    }
    public int GetHeight()
    {
        return _walls.GetLength(0);
    }

    public int GetWidth()
    {
        return _walls.GetLength(1);
    }

    public bool IsWall(int x, int y)
    {
        return _walls[x, y];
    }
}

[SuppressMessage("ReSharper", "IdentifierTypo")]
public class Character(Grid grid)
{
    private int PosX { get; set; } = 0;
    private int PosY { get; set; } = 0;
    private Direction Rotation { get; set; } = Direction.East;
    public Grid Grid { get; set; } = grid;
    public List<string> Moves { get; set; } = [];

    public Character() : this(Grid.TenSquareFalse()){}

    public void Move(int amount)
    {
        var (newx, newy) = CalcMove(amount);
        PosX = newx; PosY = newy;
    }
    public (int, int) CalcMove(int amount)
    {
        int x = PosX;
        int y = PosY;
        switch (Rotation)
        {
            case Direction.North:
                y += amount;
                break;
            case Direction.East:
                x += amount;
                break;
            case Direction.South:
                y -= amount;
                break;
            case Direction.West:
                x -= amount;
                break;
            default:
                throw new ArgumentException("Invalid direction.");
        }

        return (x, y);
    }
    
    public void Rotate(Side side)
    {
        int rot = (int)this.Rotation;
        Rotation = side switch
        {
            Side.Left => (Direction)((rot + 3) % 4),
            Side.Right => (Direction)((rot + 1) % 4),
            _ => Rotation
        };
    }

    public override string ToString()
    {
        return $"({PosX}, {PosY}) facing {Rotation.ToString().ToLower()}.";
    }
}