namespace EPainter.Core
{
    /// <summary>
    /// Representa el estado actual del intérprete E-Painter.
    /// Contiene información sobre la posición del cursor, color y tamaño del pincel.
    /// </summary>
    public class EPainterState
    {
        /// <summary>
        /// Obtiene o establece la posición X actual del cursor.
        /// </summary>
        public int X { get; set; }
        
        /// <summary>
        /// Obtiene o establece la posición Y actual del cursor.
        /// </summary>
        public int Y { get; set; }
        
        /// <summary>
        /// Obtiene o establece el color actual del pincel.
        /// </summary>
        public string BrushColor { get; set; } = "Transparent";
        
        /// <summary>
        /// Obtiene o establece el tamaño actual del pincel.
        /// </summary>
        public int BrushSize { get; set; } = 1;

        /// <summary>
        /// Inicializa una nueva instancia de la clase EPainterState con una posición inicial.
        /// </summary>
        /// <param name="x">La posición X inicial.</param>
        /// <param name="y">La posición Y inicial.</param>
        public EPainterState(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}