using System;

namespace EPainter.Core
{
    public class Canvas
    {
        public int Size;
        public string[,] Pixels;


        public Canvas(int size)
        {
            Size = size;
            Pixels = new string[size, size];
            Clear();
        }

        private void Clear()
        {
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    Pixels[x, y] = "White";
                }
            }
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }

        public void SetPixel(int x, int y, string color)
        {
            if (IsValidPosition(x, y))
            {
                Pixels[x, y] = color;
            }
        }

        public string GetPixel(int x, int y)
        {
            return IsValidPosition(x, y) ? Pixels[x, y] : null;
        }

        public void Print()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    string color = GetPixel(x, y);
                    if (color == "Black")
                        Console.Write("■ ");
                    else if (color == "White")
                        Console.Write("□ ");
                    else
                        Console.Write("  ");
                }
                Console.WriteLine();
            }
        }

    }
}