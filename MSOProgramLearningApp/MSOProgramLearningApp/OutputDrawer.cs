using Avalonia;
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
    public static Image<Rgba32> GenerateGridImage(Grid grid, int imageSize)
    {
        Image<Rgba32> image = new Image<Rgba32>(imageSize, imageSize);
        Color color = Color.Black;
        const float thicknessDevider = 5f;
        int spaceBetweenLines = imageSize / grid.GetSize();
        float thickness = spaceBetweenLines / thicknessDevider;

        
        image.Mutate(ctx =>
        {
            // background and border
            ctx.Fill(Color.White);
            ctx.Draw(color, thickness, new RectangularPolygon(0, 0, imageSize, imageSize));
            
            // grid
            for (int i = 1; i < grid.GetSize(); i++)
            {
                PointF[] verticalPoints = new PointF[2];
                verticalPoints[0] = new PointF(i * spaceBetweenLines, 0);
                verticalPoints[1] = new PointF(i * spaceBetweenLines, imageSize);
                
                PointF[] horizontalPoints = new PointF[2];
                horizontalPoints[0] = new PointF(0, i * spaceBetweenLines);
                horizontalPoints[1] = new PointF(imageSize, i * spaceBetweenLines);
                
                ctx.DrawLine(color, thickness, verticalPoints);
                ctx.DrawLine(color, thickness, horizontalPoints);
            }
        });

        //image.SaveAsBmp("grid.bmp"); //test doet output in root folder waar de .exe staat
        return image;
    }

    private static Image<Rgba32> DrawCharacter(Character c, Grid grid, int imageSize)
    {
        Image<Rgba32> image = new Image<Rgba32>(imageSize, imageSize);
        
        var uri = new Uri("avares://MSOAvaloniaApp/loopa.png");
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
                return 280f;
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