using System.Diagnostics;

namespace MSOProgramLearningApp;

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
public enum GridSquare
{
    Empty,
    Wall,
    Finish
}
public class Grid
{
    private readonly int _size;
    private readonly GridSquare[,] _grid;

    public Grid(GridSquare[,] grid)
    {
        Debug.Assert(grid.GetLength(0) == grid.GetLength(1)); // should be a square grid
        _grid = grid;
        _size = _grid.GetLength(0);
    }
    
    //generates an empty grid of size x
    public static Grid XSquareFalse(int x)
    {
        var array = new GridSquare[x, x];
        for (int i = 0; i < x; i++)
            for (int j = 0; j < x; j++)
                array[i, j] = GridSquare.Empty;
        return new Grid(array);
    }
    
    public int GetSize() => _size;

    //returns whether (x, y) is outside the grid
    public bool OutOfBounds(int x, int y)
    {
        return x < 0 || y < 0 || x > GetSize() - 1 || y > GetSize() - 1;
    }
    //returns whether (x, y) is a wall
    public bool IsWall(int x, int y)
    {
        if (OutOfBounds(x, y))
            return true;

        return _grid[x, y] == GridSquare.Wall;
    }
    //returns whether (x, y) is a finish
    public bool IsFinish(int x, int y)
    {
        if (OutOfBounds(x, y))
            return false;

        return _grid[x, y] == GridSquare.Finish;
    }
    //returns whether the grid has a finish
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
public class Character(Grid grid)
{
    private int PosX { get; set; }
    private int PosY { get; set; }
    public Direction Rotation { get; private set; } = Direction.East;
    public Grid Grid { get; } = grid;
    //saves the simple commands executed on the character (only Turn and Move)
    public List<string> Moves { get; } = [];
    //saves the points this character has been
    public readonly List<(int, int)> PointsVisited = [(0, 0)];
    public Character() : this(Grid.XSquareFalse(10)){}
    public (int, int) GetPosition()
    {
        return (PosX, PosY);
    }
    //moves the character by amount
    public void Move(int amount)
    {
        var (newx, newy) = CalcMove(amount);
        if (Grid.OutOfBounds(newx, newy))
            throw new Exception($"Moving out of bounds from {ToString()} to {newx},{newy}");
        if (Grid.IsWall(newx, newy))
            throw new Exception($"Moving into a wall  from {ToString()} to {newx},{newy}");
        PosX = newx; PosY = newy;
        PointsVisited.Add(GetPosition());
    }
    //gives the position this instance would be in after executing move by amount
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
    //rotates the character to the side specified
    public void Rotate(Side side)
    {
        int rot = (int)Rotation;
        Rotation = side switch
        {
            Side.Left => (Direction)((rot + 3) % 4),
            Side.Right => (Direction)((rot + 1) % 4),
            _ => Rotation
        };
    }

    public bool GridHasFinish() => Grid.HasFinish();
    //returns the text displayed indicating whether this instance has finished
    public string HasFinished()
    {
        if (Grid.IsFinish(PosX, PosY))
            return "Finished";
        return $"Not finished, ended on ({PosX},{PosY})";
    }
    public override string ToString()
    {
        return $"({PosX}, {PosY}) facing {Rotation.ToString().ToLower()}.";
    }
}