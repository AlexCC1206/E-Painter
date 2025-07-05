namespace EPainter.Core
{
    /// <summary>
    /// Representa una asignación de valor a una variable.
    /// </summary>
    public class Assignment : Stmt
    {
        /// <summary>
        /// Obtiene el nombre de la variable a la que se asignará el valor.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Obtiene la expresión cuyo valor se asignará a la variable.
        /// </summary>
        public Expr Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Assignment.
        /// </summary>
        /// <param name="name">El nombre de la variable.</param>
        /// <param name="value">La expresión cuyo valor se asignará.</param>
        public Assignment(string name, Expr value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de asignación.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de asignación.</param>
        /// <returns>El resultado de procesar esta sentencia de asignación.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }
    }
}