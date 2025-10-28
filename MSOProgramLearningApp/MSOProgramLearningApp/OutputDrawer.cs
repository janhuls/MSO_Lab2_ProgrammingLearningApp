using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;

namespace MSOProgramLearningApp;

// can create an image of the output given a set of instructions
public class OutputDrawer(int size = 1000) // should be a square so width and height are the same
{
    private readonly int _size = size;

    public void GenerateBitmap(List<ICommand> commands, Character character, MemoryStream outputMemStream)
    {
        Image<Rgba32> image = new Image<Rgba32>(_size, _size);
        image.Mutate(ctx =>
        {
            // draw the grid
            ctx.DrawImage(GenerateGridImage(character.Grid), 1);
            
        });
        
        // save the image to memory
        image.SaveAsBmp(outputMemStream);
        outputMemStream.Position = 0;
    }

    // generates an image of the grid
    public Image<Rgba32> GenerateGridImage(Grid grid)
    {
        Image<Rgba32> image = new Image<Rgba32>(_size, _size);
        Color color = Color.Black;
        int spaceBetweenLines = _size / grid.GetWidth();
        float thickness = spaceBetweenLines / 5f;
        
        Debug.Assert(grid.GetHeight() == grid.GetWidth()); // for now, we only support square grids

        
        image.Mutate(ctx =>
        {
            // background and border
            ctx.Fill(Color.White);
            ctx.Draw(color, thickness, new RectangularPolygon(0, 0, _size, _size));
            
            // grid
            for (int i = 1; i < grid.GetWidth(); i++)
            {
                PointF[] verticalPoints = new PointF[2];
                verticalPoints[0] = new PointF(i * spaceBetweenLines, 0);
                verticalPoints[1] = new PointF(i * spaceBetweenLines, _size);
                
                PointF[] horizontalPoints = new PointF[2];
                horizontalPoints[0] = new PointF(0, i * spaceBetweenLines);
                horizontalPoints[1] = new PointF(_size, i * spaceBetweenLines);
                
                ctx.DrawLine(color, thickness, verticalPoints);
                ctx.DrawLine(color, thickness, horizontalPoints);
            }
        });

        image.SaveAsBmp("grid.bmp"); //test doet output in root folder waar de .exe staat
        return image;
    }

    private Image<Rgba32> DrawCommand(ICommand command)
    {
        throw new NotImplementedException();
    }
}