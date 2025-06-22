namespace EPainter.Core
{
    /// <summary>
    /// Representa un token individual en el lenguaje E-Painter.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Obtiene el tipo de token.
        /// </summary>
        public TokenType Type { get; }
        
        /// <summary>
        /// Obtiene el lexema (texto original) que representa este token.
        /// </summary>
        public string Lexeme { get; }
        
        /// <summary>
        /// Obtiene el valor literal asociado con este token, si existe.
        /// </summary>
        public object Literal { get; }
        
        /// <summary>
        /// Obtiene el número de línea en el código fuente donde aparece este token.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Token.
        /// </summary>
        /// <param name="type">El tipo de token.</param>
        /// <param name="lexeme">El lexema (texto original).</param>
        /// <param name="literal">El valor literal asociado, si existe.</param>
        /// <param name="line">El número de línea en el código fuente.</param>
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }
    }
}

