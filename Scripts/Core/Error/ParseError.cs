using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que ocurre durante el análisis sintáctico del código E-Painter.
    /// </summary>
    public class ParseError : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase ParseError sin mensaje específico.
        /// </summary>
        public ParseError() : base() { }
        
        /// <summary>
        /// Inicializa una nueva instancia de la clase ParseError con un mensaje específico.
        /// </summary>
        /// <param name="message">El mensaje descriptivo del error de análisis.</param>
        public ParseError(string message) : base(message) { }
    }
}