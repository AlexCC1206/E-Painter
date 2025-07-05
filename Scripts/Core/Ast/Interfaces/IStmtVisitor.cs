namespace EPainter.Core
{
    /// <summary>
    /// Interfaz para implementar el patrón Visitor para las sentencias.
    /// </summary>
    /// <typeparam name="T">El tipo de retorno del visitante.</typeparam>
    public interface IStmtVisitor<T>
    {
        /// <summary>
        /// Visita una sentencia de asignación.
        /// </summary>
        /// <param name="stmt">La sentencia de asignación a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de asignación.</returns>
        T VisitAssignment(Assignment stmt);

        /// <summary>
        /// Visita una sentencia spawn para establecer la posición inicial.
        /// </summary>
        /// <param name="stmt">La sentencia spawn a visitar.</param>
        /// <returns>El resultado de procesar la sentencia spawn.</returns>
        T VisitSpawn(Spawn stmt);

        /// <summary>
        /// Visita una sentencia para cambiar el color de dibujo.
        /// </summary>
        /// <param name="stmt">La sentencia de cambio de color a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de cambio de color.</returns>
        T VisitColor(Color stmt);

        /// <summary>
        /// Visita una sentencia para cambiar el tamaño del pincel.
        /// </summary>
        /// <param name="stmt">La sentencia de cambio de tamaño a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de cambio de tamaño.</returns>
        T VisitSize(Size stmt);

        /// <summary>
        /// Visita una sentencia para dibujar una línea.
        /// </summary>
        /// <param name="stmt">La sentencia de dibujo de línea a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de dibujo de línea.</returns>
        T VisitDrawLine(DrawLine stmt);

        /// <summary>
        /// Visita una sentencia para dibujar un círculo.
        /// </summary>
        /// <param name="stmt">La sentencia de dibujo de círculo a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de dibujo de círculo.</returns>
        T VisitDrawCircle(DrawCircle stmt);

        /// <summary>
        /// Visita una sentencia para dibujar un rectángulo.
        /// </summary>
        /// <param name="stmt">La sentencia de dibujo de rectángulo a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de dibujo de rectángulo.</returns>
        T VisitDrawRectangle(DrawRectangle stmt);

        /// <summary>
        /// Visita una sentencia para rellenar una forma.
        /// </summary>
        /// <param name="stmt">La sentencia de relleno a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de relleno.</returns>
        T VisitFill(Fill stmt);

        /// <summary>
        /// Visita una sentencia de salto a una etiqueta.
        /// </summary>
        /// <param name="stmt">La sentencia de salto a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de salto.</returns>
        T VisitGoto(GoTo stmt);

        /// <summary>
        /// Visita una sentencia de etiqueta.
        /// </summary>
        /// <param name="stmt">La sentencia de etiqueta a visitar.</param>
        /// <returns>El resultado de procesar la sentencia de etiqueta.</returns>
        T VisitLabel(Label stmt);
    }
}