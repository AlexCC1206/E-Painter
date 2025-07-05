namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para dibujar una línea.
    /// </summary>
    public class DrawLine : Stmt
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
        /// Obtiene la expresión para la distancia de la línea.
        /// </summary>
        public Expr Distance { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase DrawLine.
        /// </summary>
        /// <param name="dirX">La expresión para la componente X de la dirección.</param>
        /// <param name="dirY">La expresión para la componente Y de la dirección.</param>
        /// <param name="distance">La expresión para la distancia de la línea.</param>
        public DrawLine(Expr dirX, Expr dirY, Expr distance)
        {
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de dibujo de línea.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de dibujo de línea.</param>
        /// <returns>El resultado de procesar esta sentencia de dibujo de línea.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawLine(this);
        }
    }
}