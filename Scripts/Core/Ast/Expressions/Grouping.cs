namespace EPainter.Core
{
    /// <summary>
    /// Representa una expresión agrupada entre paréntesis en el lenguaje E-Painter.
    /// </summary>
    public class Grouping : Expr
    {
        /// <summary>
        /// Obtiene la expresión agrupada.
        /// </summary>
        public Expr Expression { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Grouping.
        /// </summary>
        /// <param name="expression">La expresión agrupada.</param>
        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta expresión agrupada.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta expresión agrupada.</param>
        /// <returns>El resultado de procesar esta expresión agrupada.</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGrouping(this);
        }
    }
}