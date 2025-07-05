namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para dibujar un círculo.
    /// </summary>
    public class DrawCircle : Stmt
    {
        /// <summary>
        /// Obtiene la expresión para la componente X del centro relativo.
        /// </summary>
        public Expr DirX { get; }

        /// <summary>
        /// Obtiene la expresión para la componente Y del centro relativo.
        /// </summary>
        public Expr DirY { get; }

        /// <summary>
        /// Obtiene la expresión para el radio del círculo.
        /// </summary>
        public Expr Radius { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase DrawCircle.
        /// </summary>
        /// <param name="dirX">La expresión para la componente X del centro relativo.</param>
        /// <param name="dirY">La expresión para la componente Y del centro relativo.</param>
        /// <param name="radius">La expresión para el radio del círculo.</param>
        public DrawCircle(Expr dirX, Expr dirY, Expr radius)
        {
            DirX = dirX;
            DirY = dirY;
            Radius = radius;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de dibujo de círculo.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de dibujo de círculo.</param>
        /// <returns>El resultado de procesar esta sentencia de dibujo de círculo.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawCircle(this);
        }
    }
}