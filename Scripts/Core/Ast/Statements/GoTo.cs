namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para saltar a una etiqueta específica.
    /// </summary>
    public class GoTo : Stmt
    {
        /// <summary>
        /// Obtiene el nombre de la etiqueta a la que se saltará.
        /// </summary>
        public string LabelName { get; }

        /// <summary>
        /// Obtiene la expresión de condición para el salto.
        /// </summary>
        public Expr Condition { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Goto.
        /// </summary>
        /// <param name="label">El nombre de la etiqueta a la que se saltará.</param>
        /// <param name="condition">La expresión de condición para el salto.</param>
        public GoTo(string label, Expr condition)
        {
            LabelName = label;
            Condition = condition;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de salto.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de salto.</param>
        /// <returns>El resultado de procesar esta sentencia de salto.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGoto(this);
        }
    }
}