namespace EPainter.Core
{
    /// <summary>
    /// Clase base abstracta para todas las sentencias en el lenguaje E-Painter.
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// Método abstracto que implementa el patrón Visitor para procesar sentencias.
        /// </summary>
        /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
        /// <param name="visitor">El visitante que procesará esta sentencia.</param>
        /// <returns>Un resultado de tipo T producido por el visitante.</returns>
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }
}
