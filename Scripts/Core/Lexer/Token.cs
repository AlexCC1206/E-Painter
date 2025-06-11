namespace EPainter.Core
{
    /// <summary>
    /// Representa un token generado por el escáner.
    /// </summary>
    public class Token
    {

        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        /// <summary>
        /// Constructor de la clase Token.
        /// </summary>
        /// <param name="type">El tipo del token.</param>
        /// <param name="lexeme">El texto original del token.</param>
        /// <param name="literal">El valor literal asociado al token.</param>
        /// <param name="line">La línea en la que se encuentra el token.</param>
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        /// <summary>
        /// Devuelve una representación en cadena del token.
        /// </summary>
        /// <returns>Una cadena que representa el token.</returns>
        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}

