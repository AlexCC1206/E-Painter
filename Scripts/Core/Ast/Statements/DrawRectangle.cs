namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para dibujar un rectángulo.
    /// </summary>
    public class DrawRectangle : Stmt
    {
        /// <summary>
        /// Obtiene la expresión para la componente X de la dirección.
        /// </summary>
        public Expr DirX { get; }

        /// <summary>
        /// Obtiene la expresión para la componente Y de la dirección.
        /// </summary>
        public Expr DirY { get; }

        /// <summary>
        /// Obtiene la expresión para la distancia desde la posición actual.
        /// </summary>
        public Expr Distance { get; }

        /// <summary>
        /// Obtiene la expresión para el ancho del rectángulo.
        /// </summary>
        public Expr Width { get; }

        /// <summary>
        /// Obtiene la expresión para la altura del rectángulo.
        /// </summary>
        public Expr Height { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase DrawRectangle.
        /// </summary>
        /// <param name="dirX">La expresión para la componente X de la dirección.</param>
        /// <param name="dirY">La expresión para la componente Y de la dirección.</param>
        /// <param name="distance">La expresión para la distancia desde la posición actual.</param>
        /// <param name="width">La expresión para el ancho del rectángulo.</param>
        /// <param name="height">La expresión para la altura del rectángulo.</param>
        public DrawRectangle(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
        {
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de dibujo de rectángulo.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de dibujo de rectángulo.</param>
        /// <returns>El resultado de procesar esta sentencia de dibujo de rectángulo.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawRectangle(this);
        }
    }
}