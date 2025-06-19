using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que ocurre durante el análisis léxico del código E-Painter.
    /// </summary>
    public class ScannerException : EPainterException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase ScannerException.
        /// </summary>
        /// <param name="line">El número de línea donde ocurrió el error.</param>
        /// <param name="message">El mensaje descriptivo del error.</param>
        public ScannerException(int line, string message) : base(message, line) { }
        
        /// <summary>
        /// Obtiene el tipo de error específico para mostrar en mensajes.
        /// </summary>
        protected override string GetErrorType() => "Scanner Error";
    }
}