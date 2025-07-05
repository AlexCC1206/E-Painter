namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para cambiar el color de dibujo actual.
    /// </summary>
    public class Color : Stmt
    {
        /// <summary>
        /// Obtiene la expresión que representa el nombre del color.
        /// </summary>
        public Expr ColorName { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Color.
        /// </summary>
        /// <param name="colorName">La expresión para el nombre del color.</param>
        public Color(Expr colorName)
        {
            ColorName = colorName;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de cambio de color.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de cambio de color.</param>
        /// <returns>El resultado de procesar esta sentencia de cambio de color.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitColor(this);
        }
    }
}