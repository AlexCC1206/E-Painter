namespace EPainter.Core
{
    /// <summary>
    /// Enum que define los diferentes tipos de tokens.
    /// </summary>
    public enum TokenType
    {
        // Commands
        SPAWN, COLOR, SIZE, DRAW_LINE, DRAW_CIRCLE, DRAW_RECTANGLE, FILL,

        // Functions
        GET_ACTUAL_X, GET_ACTUAL_Y, GET_CANVAS_SIZE, GET_COLOR_COUNT,
        IS_BRUSH_COLOR, IS_BRUSH_SIZE, IS_CANVAS_COLOR,

        // Control Structures
        GOTO,

        // Operator
        SUM, MIN, MULT, DIV, MOD, POW,
        EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,
        AND, OR,

        // Symbols
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET, COMMA,

        // Assignment
        LEFT_ARROW,

        // Literal
        IDENTIFIER, STRING, NUMBER, COLOR_LITERAL,

        // Others
        EOF, NEWLINE,
    }
}