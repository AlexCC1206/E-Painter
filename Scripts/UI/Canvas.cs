using System;
using System.Drawing;

namespace EPainter.UI
{
    public class Canvas
    {
        public Color[,] pixels;
        public int size;

        public int Size => size;

        public Canvas(int size)
        {
            this.size = size;
            pixels = new Color[size, size];
            Clear();
        }

        private void Clear()
        {
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    pixels[x, y] = Color.White;
                }
            }
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= size || y < 0 || y >= size)
            {
                return Color.Transparent;
            }
            return pixels[x, y];
        }

        public void SetPixel(int x, int y, Color color, int brushSize = 1)
        {
            if (brushSize == 1)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    pixels[x, y] = color;
                }
                return;
            }

            int radius = brushSize / 2;
            for (var dx = -radius; dx <= radius; dx++)
            {
                for (var dy = -radius; dy < radius; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < size && ny >= 0 && ny < size)
                    {
                        pixels[nx, ny] = color;
                    }
                }
            }
        }
/*
        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(size, size);

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    bitmap.SetPixel(x, y, ToDrawingColor(pixels[x, y]));
                }
            }
        }

        private System.Drawing.Color ToDrawingColor(Color color)
        {
            return color switch
            {
                Color.Red => System.Drawing.Color.Red,
                Color.Blue => System.Drawing.Color.Blue,
                Color.Green => System.Drawing.Color.Green,
                Color.Yellow => System.Drawing.Color.Yellow,
                Color.Orange => System.Drawing.Color.Orange,
                Color.Purple => System.Drawing.Color.Purple,
                Color.Black => System.Drawing.Color.Black,
                Color.White => System.Drawing.Color.White,
                _ => System.Drawing.Color.Transparent
            };

        }*/

        public int CountColor(Color color, int x1, int y1, int x2, int y2)
        {
            int count = 0;
            for (int x = Math.Max(0, x1); x <= Math.Min(size - 1, x2); x++)
            {
                for (int y = Math.Max(0, y1); y <= Math.Min(size - 1, y2); y++)
                {
                    if (pixels[x, y] == color)
                        count++;
                }
            }
            return count;
        }
    }
}
