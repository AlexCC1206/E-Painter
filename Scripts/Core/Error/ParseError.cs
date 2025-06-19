using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que ocurre durante el análisis sintáctico del código E-Painter.
    /// </summary>
    public class ParseError : EPainterException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase ParseError sin línea específica.
        /// </summary>
        /// <param name="message">El mensaje descriptivo del error de análisis.</param>
        public ParseError(string message) : base(message) { }
        
        /// <summary>
        /// Inicializa una nueva instancia de la clase ParseError con una línea específica.
        /// </summary>
        /// <param name="message">El mensaje descriptivo del error de análisis.</param>
        /// <param name="line">El número de línea donde ocurrió el error.</param>
        public ParseError(string message, int line) : base(message, line) { }
        
        /// <summary>
        /// Obtiene el tipo de error específico para mostrar en mensajes.
        /// </summary>
        protected override string GetErrorType() => "Parse Error";
    }
}