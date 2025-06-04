namespace EPainter
{
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }

    public enum TokenType
    {
        // Commands
        SPAWN, COLOR, SIZE, DRAWLINE, DRAWCIRCLE, DRAWRECTANGLE, FILL,

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

