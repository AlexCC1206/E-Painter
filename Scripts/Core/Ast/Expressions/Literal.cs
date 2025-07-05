namespace EPainter.Core
{
    /// <summary>
    /// Representa un valor literal en el lenguaje E-Painter.
    /// </summary>
    public class Literal : Expr
    {
        /// <summary>
        /// Obtiene el valor literal.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Literal.
        /// </summary>
        /// <param name="value">El valor literal a almacenar.</param>
        public Literal(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Acepta un visitante para procesar este literal.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará este literal.</param>
        /// <returns>El resultado de procesar este literal.</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteral(this);
        }
    }
}