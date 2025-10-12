namespace ProgrammingLearningApp;

class Program
{
    private List<ICommand> program;
    private string fileName;
    private Character character;

    //TODO: 
    //add prebuilt programs
    //add metrics
    //add command line interface

    static void Main()
    {
        //some sort of command line interface
        //options for prebuilt programs or custom input file
        Program p = new Program("input");
        foreach (ICommand c in p.program)
        {
            c.Execute(p.character);
        }
        Console.WriteLine('.');
        Console.WriteLine("End state " + p.character.ToString());
    }

    public Program(string fileName)
    {
        this.fileName = fileName;
        character = new Character();
        string input = System.IO.File.ReadAllText(fileName);
        program = ParseInput(input);
    }

    private List<ICommand> ParseInput(string s)
    {
        string[] lines = s.Split('\n');
        List<ICommand> commands = new List<ICommand>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.StartsWith("Repeat"))
            {
                ParseRepeat(lines, ref i, 4);
            }
            else
            {
                ParseSimpleCommand(line.Split(' '));
            }
        }

        return commands;
    }

    private ICommand ParseRepeat(string[] lines, ref int index, int depth) //depth is the current spacing expected before repeat block
    {
        string line = lines[index];
        int times = int.Parse(line.Split(' ')[1]);
        index++;
        List<ICommand> repeatCommands = new List<ICommand>();

        while (index < lines.Length && lines[index].StartsWith(new string(' ', depth)))
        {
            string trimmedLine = lines[index].Trim();
            if (trimmedLine.StartsWith("Repeat"))
            {
                repeatCommands.Add(ParseRepeat(lines, ref index, depth + 4));
            }
            else
            {
                repeatCommands.Add(ParseSimpleCommand(trimmedLine.Split(' ')));
            }
            index++;
        }

        return new Repeat(times, repeatCommands);
    }

    private ICommand ParseSimpleCommand(string[] s)
    {
        try
        {
            switch (s[0])
            {
                case "Turn":
                    SIDE side = (SIDE)Enum.Parse(typeof(SIDE), s[1], true);
                    return new Turn(side);
                case "Move":
                    int amount = int.Parse(s[1]);
                    return new Move(amount);
                default:
                    throw new Exception("Unknown command: " + s[0]);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error parsing command: " + string.Join(" ", s) + "\n" + e.Message);
        }
    }
}