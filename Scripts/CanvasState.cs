using System;
using System.Drawing;

namespace EPainter
{
    public class WallEState
    {
        public int X;
        public int Y;
        public Color CurrentColor = Color.Transparent;
        public int BrushSize = 1;
    }
    public class CanvasState
    {
        public Color[,] Pixels;
        public int Size;

        public CanvasState(int size)
        {
            Size = size;
            Pixels = new Color[size, size];
            Clear();
        }

        private void Clear()
        {
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    Pixels[x, y] = Color.White;
                }
            }
        }


        public void Spawn(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size)
            {
                throw new Exception("Initial position off the canvas");
            }
        }

        public void DrawLine(int dirX, int dirY, int distance)
        {
            for (var i = 0; i < distance; i++)
            {

            }
        }

        public void DrawCircle(int dirX, int dirY, int radius)
        {

        }

        public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {

        }

        public void Fill()
        {

        }

        public void Color1(Color color)
        {

        }

        public void Sizes(int k)
        {
    
        }

    }
}
