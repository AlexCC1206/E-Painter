using System;

namespace EPainter.Core
{
    /// <summary>
    /// Clase base para todas las excepciones en E-Painter.
    /// </summary>
    public abstract class EPainterException : Exception
    {
        /// <summary>
        /// Obtiene el número de línea donde ocurrió el error (si está disponible).
        /// </summary>
        public int? Line { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase EPainterException.
        /// </summary>
        /// <param name="message">El mensaje descriptivo del error.</param>
        /// <param name="line">El número de línea donde ocurrió el error (opcional).</param>
        protected EPainterException(string message, int? line = null) : base(message)
        {
            Line = line;
        }
        
        /// <summary>
        /// Devuelve una representación en cadena de este error.
        /// </summary>
        /// <returns>Una cadena con la información del error.</returns>
        public override string ToString()
        {
            return Line.HasValue 
                ? $"[Line {Line}] {GetErrorType()}: {Message}" 
                : $"{GetErrorType()}: {Message}";
        }

        /// <summary>
        /// Obtiene el tipo de error específico para mostrar en mensajes.
        /// </summary>
        protected abstract string GetErrorType();
    }
}