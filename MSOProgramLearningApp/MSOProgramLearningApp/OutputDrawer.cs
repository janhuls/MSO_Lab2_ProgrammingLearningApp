using System.Diagnostics;
using Avalonia.Controls.Shapes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing.Processors;
using Point = Avalonia.Point;

namespace MSOProgramLearningApp;

// can create an image of the output given a set of instructions
public class OutputDrawer(int imageSize = 1000) // should be a square so width and height are the same
{
    private readonly int _imageSize = imageSize;

    public void GenerateBitmap(List<ICommand> commands, Character character, MemoryStream outputMemStream)
    {
        Image<Rgba32> image = new Image<Rgba32>(_imageSize, _imageSize);
        image.Mutate(ctx =>
        {
            // draw the grid
            ctx.DrawImage(GenerateGridImage(character.Grid), 1);
            //ctx.DrawImage( DrawPath())
        });
        
        // save the image to memory
        image.SaveAsBmp(outputMemStream);
        outputMemStream.Position = 0;
    }

    // generates an image of the grid
    public Image<Rgba32> GenerateGridImage(Grid grid)
    {
        Image<Rgba32> image = new Image<Rgba32>(_imageSize, _imageSize);
        Color color = Color.Black;
        int spaceBetweenLines = _imageSize / grid.GetSize();
        float thickness = spaceBetweenLines / 5f;

        
        image.Mutate(ctx =>
        {
            // background and border
            ctx.Fill(Color.White);
            ctx.Draw(color, thickness, new RectangularPolygon(0, 0, _imageSize, _imageSize));
            
            // grid
            for (int i = 1; i < grid.GetSize(); i++)
            {
                PointF[] verticalPoints = new PointF[2];
                verticalPoints[0] = new PointF(i * spaceBetweenLines, 0);
                verticalPoints[1] = new PointF(i * spaceBetweenLines, _imageSize);
                
                PointF[] horizontalPoints = new PointF[2];
                horizontalPoints[0] = new PointF(0, i * spaceBetweenLines);
                horizontalPoints[1] = new PointF(_imageSize, i * spaceBetweenLines);
                
                ctx.DrawLine(color, thickness, verticalPoints);
                ctx.DrawLine(color, thickness, horizontalPoints);
            }
        });

        //image.SaveAsBmp("grid.bmp"); //test doet output in root folder waar de .exe staat
        return image;
    }

    private Image<Rgba32> DrawPath(List<(int,int)> points, float thickness)
    {
        Image<Rgba32> image = new Image<Rgba32>(_imageSize, _imageSize);

        Color color = Color.Blue;
        
        image.Mutate(ctx =>
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                PointF[] twoPoints = new PointF[2];
                twoPoints[0] = new PointF(points[i].Item1, points[i].Item2);
                twoPoints[1] = new PointF(points[i+1].Item1, points[i+1].Item2);
                ctx.DrawLine(color, thickness, twoPoints);
            }
        });

        return image;
    }
}