using System.Diagnostics;

namespace MSOProgramLearningApp;
public interface IParser
{
    public List<ICommand> Parse();
}
public static class GridParser
{
    public static Grid Parse(string input)
    {
        return new Grid(ArrayParse(input));
    }
    private static GridSquare[,] ArrayParse(string input)
    {
        var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) return new GridSquare[0,0];

        int rows = lines.Length;
        int cols = lines[0].Length;
        var grid = new GridSquare[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            if (lines[i].Length != cols)
                throw new ArgumentException("All lines must have the same length.");

            for (int j = 0; j < cols; j++)
            {
                char c = lines[i][j];
                grid[i, j] = c switch
                {
                    '+' => GridSquare.Wall,
                    'o' => GridSquare.Empty,
                    'x' => GridSquare.Finish,
                    _ => throw new ArgumentException($"Invalid character '{c}' at line {i+1}, column {j+1}")
                };
            }
        }

        return grid;
    }
}
public class StringParser(string program) : IParser
{
    public const int IndentSize = 4;

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
                commands.Add(ParseRepeatable(lines, ref index, expectedIndent));
            else
            {
                commands.Add(ParseSimpleCommand(trimmed));
                index++;
            }
        }
        return commands;
    }

    private Repeatable ParseRepeatable(string[] lines, ref int index, int expectedIndent)
    {
        string line = lines[index].Trim();
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new Exception($"Invalid repeat syntax at line {index + 1}: '{line}'");

        string keyword = parts[0];
        string arg = parts[1];

        // RepeatUntil
        if (keyword.Equals("RepeatUntil", StringComparison.OrdinalIgnoreCase))
        {
            var condition = ParseCondition(arg);

            index++;
            var innerCommands = ParseBlock(lines, ref index, expectedIndent + IndentSize);

            return new ConditionalRepeat(innerCommands, condition);
        }

        // regular Repeat
        if (!int.TryParse(arg, out int times))
            throw new Exception($"Invalid repeat count at line {index + 1}: '{line}'");

        index++;
        var repeatCommands = ParseBlock(lines, ref index, expectedIndent + IndentSize);
        return new Repeat(times, repeatCommands);
    }

    private ICondition ParseCondition(string token)
    {
        return token.Equals("WallAhead", StringComparison.OrdinalIgnoreCase)
            ? new WallAhead()
            : new GridEdge();
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
    private int _size;
    private readonly GridSquare[,] _grid;

    public Grid(GridSquare[,] grid)
    {
        Debug.Assert(grid.GetLength(0) == grid.GetLength(1)); // should be a square grid
        _grid = grid;
        _size = _grid.GetLength(0);
    }

    public static Grid XSquareFalse(int x)
    {
        var array = new GridSquare[x, x];
        for (int i = 0; i < x; i++)
            for (int j = 0; j < x; j++)
                array[i, j] = GridSquare.Empty;
        return new Grid(array);
    }

    public int GetSize() => _size;

    public bool outOfBounds(int x, int y)
    {
        return x < 0 || y < 0 || x > GetSize() - 1 || y > GetSize() - 1;
    }
    public bool IsWall(int x, int y)
    {
        if (outOfBounds(x, y))
            return true;

        return _grid[x, y] == GridSquare.Wall;
    }

    public bool IsFinish(int x, int y)
    {
        if (outOfBounds(x, y))
            return false;

        return _grid[x, y] == GridSquare.Finish;
    }

    public bool HasFinish()
    {
        for (int i = 0; i < GetSize(); i++)
            for (int j = 0; j < GetSize(); j++)
                if (_grid[i, j] == GridSquare.Finish)
                    return true;
        return false;
    }

    public GridSquare GetCellState(int x, int y) => _grid[x, y];
}

public enum GridSquare
{
    Empty,
    Wall,
    Finish
}
public class Character(Grid grid)
{
    private int PosX { get; set; }
    private int PosY { get; set; }
    public Direction Rotation { get; private set; } = Direction.East;
    public Grid Grid { get; } = grid;
    public List<string> Moves { get; } = [];
    public List<(int, int)> PointsVisited = [(0, 0)];
    public Character() : this(Grid.XSquareFalse(10)){}
    public (int, int) GetPosition()
    {
        return (PosX, PosY);
    }
    public void Move(int amount)
    {
        var (newx, newy) = CalcMove(amount);
        if (grid.outOfBounds(newx, newy))
            throw new Exception($"Moving out of bounds from {ToString()} to {newx},{newy}");
        PosX = newx; PosY = newy;
        PointsVisited.Add(GetPosition());
    }
    public (int, int) CalcMove(int amount)
    {
        int x = PosX;
        int y = PosY;
        switch (Rotation)
        {
            case Direction.North:
                y -= amount;
                break;
            case Direction.East:
                x += amount;
                break;
            case Direction.South:
                y += amount;
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

    public bool GridHasFinish()
    {
        return grid.HasFinish();
    }
    public string HasFinished()
    {
        if (grid.IsFinish(PosX, PosY))
            return "Finished";
        return $"Not finished, ended on ({PosX},{PosY})";
    }
    public override string ToString()
    {
        return $"({PosX}, {PosY}) facing {Rotation.ToString().ToLower()}.";
    }
}