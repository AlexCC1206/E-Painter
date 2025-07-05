namespace EPainter.Core
{
    /// <summary>
    /// Representa una expresión unaria en el lenguaje E-Painter.
    /// </summary>
    public class Unary : Expr
    {
        /// <summary>
        /// Obtiene el token del operador.
        /// </summary>
        public Token Op { get; }

        /// <summary>
        /// Obtiene la expresión operando.
        /// </summary>
        public Expr Right { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Unary.
        /// </summary>
        /// <param name="op">El token del operador.</param>
        /// <param name="right">La expresión operando.</param>
        public Unary(Token op, Expr right)
        {
            Op = op;
            Right = right;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta expresión unaria.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta expresión unaria.</param>
        /// <returns>El resultado de procesar esta expresión unaria.</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnary(this);
        }
    }
}