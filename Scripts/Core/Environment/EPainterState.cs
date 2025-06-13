namespace EPainter.Core
{
    public class EPainterState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string BrushColor { get; set; } = "Transparent";
        public int BrushSize { get; set; } = 1;

        public EPainterState(int x, int y)
        {
            X = X;
            Y = Y;
        }
    }
}