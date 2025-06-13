namespace EPainter.Core
{
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
        EQUAL_EQUAL, BANG_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,
        AND, OR,

        // Symbols
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET, COMMA,

        // Assignment
        ARROW,

        // Literal
        IDENTIFIER, NUMBER, COLOR_LITERAL,

        // Boolean Literals
        TRUE, FALSE,

        // End of file
        EOF,

        // Jumpline
        NEWLINE,
    }
}