namespace ProgrammingLearningApp;

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