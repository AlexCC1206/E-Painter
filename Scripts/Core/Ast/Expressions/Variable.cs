namespace EPainter.Core
{
    /// <summary>
    /// Representa una referencia a una variable en el lenguaje E-Painter.
    /// </summary>
    public class Variable : Expr
    {
        /// <summary>
        /// Obtiene el nombre de la variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Variable.
        /// </summary>
        /// <param name="name">El nombre de la variable.</param>
        public Variable(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta variable.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta variable.</param>
        /// <returns>El resultado de procesar esta variable.</returns>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}