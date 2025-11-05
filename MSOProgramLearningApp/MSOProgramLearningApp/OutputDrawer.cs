using Avalonia.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;

namespace MSOProgramLearningApp;

// can create an image of the output given a set of instructions
public static class OutputDrawer
{
    //generates the grid, character and path, saves it to memory
    public static void GenerateBitmap(Character character, MemoryStream outputMemStream, int imageSize = 1000)
    {
        var image = new Image<Rgba32>(imageSize, imageSize); // should be a square
        image.Mutate(ctx =>
        {
            var p = character.PointsVisited;
            var g = character.Grid;
            // draw the grid
            ctx.DrawImage(GenerateGridImage(g, imageSize), 1);
            ctx.DrawImage(DrawPath(p, g, imageSize), 1);
            ctx.DrawImage(GenerateCharacterImage(character, g, imageSize), 1);
        });

        // save the image to memory
        image.SaveAsBmp(outputMemStream);
        outputMemStream.Position = 0;
    }

    // generates an image of the grid
    private static Image<Rgba32> GenerateGridImage(Grid grid, int imageSize)
    {
        var gridSize = grid.GetSize();
        var cellSize = imageSize / gridSize;
        var lineThickness = cellSize / 5f;
        var gridColor = Color.Black;
        var backgroundColor = Color.White;

        var image = new Image<Rgba32>(imageSize, imageSize);

        image.Mutate(ctx =>
        {
            // Draw background
            ctx.Fill(backgroundColor);

            DrawCells(ctx, gridSize, cellSize, grid);

            DrawGridLines(ctx, gridSize, cellSize, gridColor, lineThickness, imageSize);
        });

        return image;
    }

    //draw the cells on the given ctx
    private static void DrawCells(IImageProcessingContext ctx, int gridSize, int cellSize, Grid grid)
    {
        //loop over grid
        for (var row = 0; row < gridSize; row++)
        for (var col = 0; col < gridSize; col++)
        {
            var cellColor = GetCellColor(grid.GetCellState(row, col));

            float x = col * cellSize;
            float y = row * cellSize;

            var rect = new RectangularPolygon(x, y, cellSize, cellSize);
            ctx.Fill(cellColor, rect);
        }
    }

    //returns cell color based on GridSquare
    private static Color GetCellColor(GridSquare cell)
    {
        return cell switch
        {
            GridSquare.Empty => Color.White,
            GridSquare.Wall => Color.Red,
            GridSquare.Finish => Color.Green,
            _ => throw new ArgumentException($"Could not get cell color, {cell} is not valid")
        };
    }

    //draws grid lines on the given ctx
    private static void DrawGridLines(IImageProcessingContext ctx, int gridSize, int cellSize, Color color,
        float thickness, int imageSize)
    {
        //draw vertical linesG
        for (var i = 0; i <= gridSize; i++)
        {
            float x = i * cellSize;
            ctx.DrawLine(color, thickness, new PointF(x, 0), new PointF(x, imageSize));
        }

        //draw horizontal lines
        for (var i = 0; i <= gridSize; i++)
        {
            float y = i * cellSize;
            ctx.DrawLine(color, thickness, new PointF(0, y), new PointF(imageSize, y));
        }
    }

    //generates the image for the character
    private static Image<Rgba32> GenerateCharacterImage(Character c, Grid grid, int imageSize)
    {
        var image = new Image<Rgba32>(imageSize, imageSize);

        using var sprite = GetCharacterSprite();

        //resize sprite to fit in cells
        var cellSize = imageSize / (float)grid.GetSize();
        sprite.Mutate(x => x.Resize((int)cellSize, (int)cellSize));

        //find position for the sprite
        var position = getPointOnGrid(c.GetPosition(), grid, imageSize);
        var x = position.X - sprite.Width / 2f;
        var y = position.Y - sprite.Height / 2f;

        //starts facing down, makes it face the correct way
        sprite.Mutate(ctx => ctx.Rotate(GetAngleFromDirection(c.Rotation)));

        image.Mutate(ctx => { ctx.DrawImage(sprite, new Point((int)x, (int)y), 1f); });

        return image;
    }

    //returns the character sprite
    private static Image<Rgba32> GetCharacterSprite()
    {
        var uri = new Uri("avares://MSOAvaloniaApp/Assets/loopa.png");
        using var stream = AssetLoader.Open(uri);

        return stream == null ? throw new InvalidOperationException($"Could not find Avalonia resource at {uri}") : Image.Load<Rgba32>(stream);
    }

    //gets the angle offset
    private static float GetAngleFromDirection(Direction d)
    {
        //offset from 1 1/2 pi radians
        return d switch
        {
            Direction.East => 270f,
            Direction.South => 0f,
            Direction.West => 90f,
            Direction.North => 180f,
            _ => throw new ArgumentException("Invalid direction")
        };
    }

    //draws the path the character has walked
    private static Image<Rgba32> DrawPath(List<(int, int)> points, Grid grid, int imageSize)
    {
        var image = new Image<Rgba32>(imageSize, imageSize);

        var color = Color.Blue;
        const float thicknessDivider = 4f;

        var thickness = imageSize / (thicknessDivider * grid.GetSize());

        image.Mutate(ctx =>
        {
            //loops over points visited
            for (var i = 0; i < points.Count - 1; i++)
            {
                var twoPoints = new PointF[2];
                twoPoints[0] = getPointOnGrid(points[i], grid, imageSize);
                twoPoints[1] = getPointOnGrid(points[i + 1], grid, imageSize);
                ctx.DrawLine(color, thickness, twoPoints);
            }
        });

        return image;
    }

    //returns the world coordinates of a grid pointG
    private static PointF getPointOnGrid((int, int) pt, Grid grid, int imageSize)
    {
        var spaceBetweenLines = imageSize / grid.GetSize();
        var offset = spaceBetweenLines / 2f;
        var (x, y) = pt;

        return new PointF(x * spaceBetweenLines + offset, y * spaceBetweenLines + offset);
    }
}