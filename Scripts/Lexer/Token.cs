namespace EPainter
{
    public class Token
{
    public TokenType type;
    public string lexeme;
    public object literal;
    public int line;

    public Token(TokenType type, string lexeme ,object literal ,int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public override string ToString()
    {
        return $"{type} {lexeme} {literal}";
    }
}

    public enum TokenType
    {
        // Keywords
        SPAWN, COLOR, SIZE, DRAWLINE, DRAWCIRCLE, DRAWRECTANGLE, FILL,
        GETACTUALX, GETACTUALY, GETCANVASIZE, GETCOLORCOUNT,
        ISBRUSHCOLOR, ISBRUSHSIZE, ISCANVASCOLOR,
    
        // Control Structures
        GOTO, LABEL,

        // Operator
        LEFT_PAREN, RIGHT_PAREN, COMMA,
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
        EOF,
        NEWLINE
    }
}

