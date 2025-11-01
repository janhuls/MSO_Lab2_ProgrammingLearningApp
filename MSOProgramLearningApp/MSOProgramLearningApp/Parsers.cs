namespace MSOProgramLearningApp;

//parses a grid from the specified input to a Grid object
public static class GridParser
{
    //parse to grid
    public static Grid Parse(string input) => new(ArrayParse(input));
    //parse string to grid
    private static GridSquare[,] ArrayParse(string input)
    {
        var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length == 0) return new GridSquare[0,0]; //empty grid
        
        int rows = lines.Length;
        int cols = lines[0].Length;
        var grid = new GridSquare[rows, cols];

        //loop over rows and cols
        for (int i = 0; i < rows; i++)
        {
            if (lines[i].Length != cols)
                throw new ArgumentException("All lines must have the same length.");

            for (int j = 0; j < cols; j++)
            {
                //parse character at position
                char c = lines[i][j];
                try
                {
                    grid[i,j] = CellParse(c);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.Message + $"at line {i + 1}, column {j + 1}");
                }
            }
        }
        return grid;
    }
    //convert Char to GridSquare
    private static GridSquare CellParse(Char s)
    {
        switch (s)
        {
            case '+':
                return GridSquare.Wall;
            case 'o':
                return GridSquare.Empty;
            case 'x':
                return GridSquare.Finish;
            default:
                throw new ArgumentException($"Invalid character '{s}'");
        }
    }
}
public interface IParser
{
    public List<ICommand> Parse();
}
//parses a List<ICommand> from a string in the specified format
public class StringParser(string program) : IParser
{
    //default size for indentation in repeatable blocks
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
    
    //parses a cleaned up string[] 
    private List<ICommand> ParseBlock(string[] lines, ref int index, int expectedIndent)
    {
        List<ICommand> commands = [];
        
        //loop over lines to parse
        while (index < lines.Length)
        {
            string line = lines[index];
            int indent = ParserUtils.CountLeadingSpaces(line);

            if (indent < expectedIndent) //end of repeatable block
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

    //parses a repeatable command
    private Repeatable ParseRepeatable(string[] lines, ref int index, int expectedIndent)
    {
        string line = lines[index].Trim();
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new Exception($"Invalid repeat syntax at line {index + 1}: '{line}'");

        string keyword = parts[0];
        string arg = parts[1];

        //RepeatUntil
        if (keyword.Equals("RepeatUntil", StringComparison.OrdinalIgnoreCase))
        {
            var condition = ParseCondition(arg);

            index++;
            //gives the increased expectedIndent to parse the inner block
            var innerCommands = ParseBlock(lines, ref index, expectedIndent + IndentSize);

            return new ConditionalRepeat(innerCommands, condition);
        }

        //regular Repeat
        if (!int.TryParse(arg, out int times))
            throw new Exception($"Invalid repeat count at line {index + 1}: '{line}'");

        index++;
        //gives the increase expectedIndent to parse inner block
        var repeatCommands = ParseBlock(lines, ref index, expectedIndent + IndentSize);
        return new Repeat(times, repeatCommands);
    }
    //parses a condition
    private ICondition ParseCondition(string token)
    {
        //only 2 cases so no switch needed
        return token.Equals("WallAhead", StringComparison.OrdinalIgnoreCase)
            ? new WallAhead() : new GridEdge();
    }
    //parse a Move or Turn command
    private static ICommand ParseSimpleCommand(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return CreateCommand(parts);
    }
    //returns a Move or Turn instance
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
    //count the leading spaces of a string
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
