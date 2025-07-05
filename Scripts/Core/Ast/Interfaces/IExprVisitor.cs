namespace EPainter.Core
{
    /// <summary>
    /// Interfaz para implementar el patrón Visitor para las expresiones.
    /// </summary>
    /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
    public interface IExprVisitor<T>
    {
        /// <summary>
        /// Visita una expresión literal.
        /// </summary>
        /// <param name="expr">La expresión literal a visitar.</param>
        /// <returns>El resultado de procesar la expresión literal.</returns>
        T VisitLiteral(Literal expr);

        /// <summary>
        /// Visita una expresión de variable.
        /// </summary>
        /// <param name="expr">La expresión de variable a visitar.</param>
        /// <returns>El resultado de procesar la expresión de variable.</returns>
        T VisitVariable(Variable expr);

        /// <summary>
        /// Visita una expresión unaria.
        /// </summary>
        /// <param name="expr">La expresión unaria a visitar.</param>
        /// <returns>El resultado de procesar la expresión unaria.</returns>
        T VisitUnary(Unary expr);

        /// <summary>
        /// Visita una expresión binaria.
        /// </summary>
        /// <param name="expr">La expresión binaria a visitar.</param>
        /// <returns>El resultado de procesar la expresión binaria.</returns>
        T VisitBinary(Binary expr);

        /// <summary>
        /// Visita una expresión de agrupación.
        /// </summary>
        /// <param name="expr">La expresión de agrupación a visitar.</param>
        /// <returns>El resultado de procesar la expresión de agrupación.</returns>
        T VisitGrouping(Grouping expr);

        /// <summary>
        /// Visita una expresión de llamada a función.
        /// </summary>
        /// <param name="expr">La expresión de llamada a función a visitar.</param>
        /// <returns>El resultado de procesar la expresión de llamada a función.</returns>
        T VisitCall(Call expr);
    }
}