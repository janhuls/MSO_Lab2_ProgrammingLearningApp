using Avalonia.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using Point = SixLabors.ImageSharp.Point;

namespace MSOProgramLearningApp;

// can create an image of the output given a set of instructions
public static class OutputDrawer 
{
    public static void GenerateBitmap(Character character, MemoryStream outputMemStream, int imageSize = 1000)
    {
        Image<Rgba32> image = new Image<Rgba32>(imageSize, imageSize); // should be a square
        image.Mutate(ctx =>
        {
            var p = character.PointsVisited;
            var g = character.Grid;
            // draw the grid
            ctx.DrawImage(GenerateGridImage(g, imageSize), 1);
            ctx.DrawImage(DrawPath(p, g, imageSize), 1);
            ctx.DrawImage(DrawCharacter(character, g, imageSize), 1);
        });
        
        // save the image to memory
        image.SaveAsBmp(outputMemStream);
        outputMemStream.Position = 0;
    }

    // generates an image of the grid
    private static Image<Rgba32> GenerateGridImage(Grid grid, int imageSize)
    {
        int gridSize = grid.GetSize();
        int cellSize = imageSize / gridSize;
        float lineThickness = cellSize / 5f;
        Color gridColor = Color.Black;
        Color backgroundColor = Color.White;

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

    private static void DrawCells(IImageProcessingContext ctx, int gridSize, int cellSize, Grid grid)
    {
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Color cellColor;
                switch (grid.GetCellState(col, row))
                {
                    case GridSquare.Empty:
                        cellColor = Color.White;
                        break;
                    case GridSquare.Wall:
                        cellColor = Color.Red;
                        break;
                    case GridSquare.Finish:
                        cellColor = Color.Green;
                        break;
                    default:
                        cellColor = Color.RebeccaPurple;
                        break;
                }

                float x = col * cellSize;
                float y = row * cellSize;

                var rect = new RectangularPolygon(x, y, cellSize, cellSize);
                ctx.Fill(cellColor, rect);
            }
        }
    }

    private static void DrawGridLines(IImageProcessingContext ctx, int gridSize, int cellSize, Color color, float thickness, int imageSize)
    {
        // Draw vertical lines
        for (int i = 0; i <= gridSize; i++)
        {
            float x = i * cellSize;
            ctx.DrawLine(color, thickness, new PointF(x, 0), new PointF(x, imageSize));
        }

        // Draw horizontal lines
        for (int i = 0; i <= gridSize; i++)
        {
            float y = i * cellSize;
            ctx.DrawLine(color, thickness, new PointF(0, y), new PointF(imageSize, y));
        }
    }

    private static Image<Rgba32> DrawCharacter(Character c, Grid grid, int imageSize)
    {
        Image<Rgba32> image = new Image<Rgba32>(imageSize, imageSize);
        
        var uri = new Uri("avares://MSOAvaloniaApp/Assets/loopa.png");
        using Stream? stream = AssetLoader.Open(uri);
        
        if (stream == null)
            throw new InvalidOperationException($"Could not find Avalonia resource at {uri}");

        using var sprite = Image.Load<Rgba32>(stream);
        
        float cellSize = imageSize / (float)grid.GetSize();
        sprite.Mutate(x => x.Resize((int)cellSize, (int)cellSize));
        
        PointF position = getPointOnGrid(c.GetPosition(), grid,imageSize);
        float x = position.X - sprite.Width / 2f;
        float y = position.Y - sprite.Height / 2f;

        // starts facing down
        sprite.Mutate(ctx => ctx.Rotate(GetAngleFromDirection(c.Rotation)));
        
        image.Mutate(ctx =>
        {
            ctx.DrawImage(sprite, new Point((int)x, (int)y), 1f); // 1f = full opacity
        });

        return image;
    }

    private static float GetAngleFromDirection(Direction d)
    {
        // south as 0, west as 90
        switch (d)
        {
            case Direction.East:
                return 270f;
            case Direction.South:
                return 0f;
            case Direction.West:
                return 90f;
            case Direction.North:
                return 180f;
            default:
                throw new ArgumentException("Invalid direction");
        }
    }
    private static Image<Rgba32> DrawPath(List<(int,int)> points, Grid grid, int imageSize)
    {
        Image<Rgba32> image = new Image<Rgba32>(imageSize, imageSize);

        Color color = Color.Blue;
        const float thicknessDevider = 4f;

        // ReSharper disable once PossibleLossOfFraction
        float thickness = (imageSize / grid.GetSize()) / thicknessDevider;
        
        image.Mutate(ctx =>
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                PointF[] twoPoints = new PointF[2];
                twoPoints[0] = getPointOnGrid(points[i], grid,imageSize);
                twoPoints[1] = getPointOnGrid(points[i + 1], grid,imageSize);
                ctx.DrawLine(color, thickness, twoPoints);
            }
        });

        return image;
    }

    private static PointF getPointOnGrid((int,int) pt, Grid grid, int imageSize)
    {
        int spaceBetweenLines = imageSize / grid.GetSize();
        float offset = spaceBetweenLines / 2f;
        var (x, y) = pt;
        
        return new PointF(x * spaceBetweenLines + offset, y * spaceBetweenLines + offset);
    }
}