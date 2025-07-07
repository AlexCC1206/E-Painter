using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que ocurre durante el análisis sintáctico del código E-Painter.
    /// </summary>
    public class ParseError : Exception
    {
        public ParseError(string message, int line) : base(message)
        {
        }

    }
}