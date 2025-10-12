namespace ProgrammingLearningApp;

enum DIRECTION
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}
enum SIDE
{
    LEFT,
    RIGHT
}

public class Character 
{
    private int posX { get; set; }
    private int posY { get; set; }

    private DIRECTION rotation { get; set; }

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
}

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
        c.rotate(side);
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
        for (int i = 0; i < times; i++)
        {
            foreach (var command in commands)
            {
                command.Execute(c);
            }
        }
    }
}