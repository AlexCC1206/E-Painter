namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para cambiar el tamaño del pincel.
    /// </summary>
    public class Size : Stmt
    {
        /// <summary>
        /// Obtiene la expresión que representa el valor del tamaño.
        /// </summary>
        public Expr SizeValue { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Size.
        /// </summary>
        /// <param name="sizeValue">La expresión para el valor del tamaño.</param>
        public Size(Expr sizeValue)
        {
            SizeValue = sizeValue;
        }

        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de cambio de tamaño.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de cambio de tamaño.</param>
        /// <returns>El resultado de procesar esta sentencia de cambio de tamaño.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitSize(this);
        }
    }
}