using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que ocurre durante el análisis léxico del código E-Painter.
    /// </summary>
    public class ScannerException : Exception
    {
        /// <summary>
        /// Obtiene el número de línea donde ocurrió el error de escaneo.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase ScannerException con una línea y mensaje específicos.
        /// </summary>
        /// <param name="line">El número de línea donde ocurrió el error.</param>
        /// <param name="message">El mensaje descriptivo del error de escaneo.</param>
        public ScannerException(int line, string message) : base(message)
        {
            Line = line;
        }

        /// <summary>
        /// Devuelve una representación en cadena de este error de escaneo.
        /// </summary>
        /// <returns>Una cadena con la información de línea y mensaje del error.</returns>
        public override string ToString()
        {
            return $"[Line {Line}] Scanner Error: {Message}";
        }
    }
}