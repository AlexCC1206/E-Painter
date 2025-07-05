namespace EPainter.Core
{
    /// <summary>
    /// Representa una expresión binaria en el lenguaje E-Painter.
    /// </summary>
    public class Binary : Expr
    {
        /// <summary>
        /// Obtiene la expresión del lado izquierdo.
        /// </summary>
        public Expr Left { get; }

        /// <summary>
        /// Obtiene el token del operador.
        /// </summary>
        public Token Op { get; }

        /// <summary>
        /// Obtiene la expresión del lado derecho.
        /// </summary>
        public Expr Right { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Binary.
        /// </summary>
        /// <param name="left">La expresión del lado izquierdo.</param>
        /// <param name="op">El token del operador.</param>
        /// <param name="right">La expresión del lado derecho.</param>
        public Binary(Expr left, Token op, Expr right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta expresión binaria.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta expresión binaria.</param>
        /// <returns>El resultado de procesar esta expresión binaria.</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitBinary(this);
        }
    }
}