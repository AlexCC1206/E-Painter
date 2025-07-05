namespace EPainter.Core
{
    /// <summary>
    /// Representa la sentencia para rellenar una forma cerrada.
    /// </summary>
    public class Fill : Stmt
    {
        /// <summary>
        /// Acepta un visitante para procesar esta sentencia de relleno.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia de relleno.</param>
        /// <returns>El resultado de procesar esta sentencia de relleno.</returns>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFill(this);
        }
    }
}