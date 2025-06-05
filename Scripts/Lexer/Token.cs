namespace EPainter
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

    /// <summary>
    /// Enum que define los diferentes tipos de tokens.
    /// </summary>
    public enum TokenType
    {
        // Commands
        SPAWN, COLOR, SIZE, DRAW_LINE, DRAW_CIRCLE, DRAW_RECTANGLE, FILL,

        // Functions
        GETACTUALX, GETACTUALY, GETCANVASIZE, GETCOLORCOUNT,
        ISBRUSHCOLOR, ISBRUSHSIZE, ISCANVASCOLOR,

        // Control Structures
        GOTO,

        // Operator
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET, COMMA,
        SUM, MIN, MULT, DIV, MOD, POW,
        EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,
        AND, OR,


        // Assignment
        LEFT_ARROW,

        // Literal
        IDENTIFIER, STRING, NUMBER,

        // Colors
        RED, BLUE, GREEN, YELLOW, ORANGE, PURPLE, BLACK, WHITE, TRANSPARENT,

        // Others
        EOF, NEWLINE,
    }
}

