namespace EPainter.Core
{
    /// <summary>
    /// Enumeración que define todos los tipos de tokens reconocidos en el lenguaje E-Painter.
    /// </summary>
    public enum TokenType
    {
        // Commands
        /// <summary>Comando para establecer la posición inicial.</summary>
        SPAWN, 
        /// <summary>Comando para establecer el color del pincel.</summary>
        COLOR, 
        /// <summary>Comando para establecer el tamaño del pincel.</summary>
        SIZE, 
        /// <summary>Comando para dibujar una línea.</summary>
        DRAW_LINE, 
        /// <summary>Comando para dibujar un círculo.</summary>
        DRAW_CIRCLE, 
        /// <summary>Comando para dibujar un rectángulo.</summary>
        DRAW_RECTANGLE, 
        /// <summary>Comando para rellenar una forma cerrada.</summary>
        FILL,

        // Functions
        /// <summary>Función para obtener la coordenada X actual.</summary>
        GET_ACTUAL_X, 
        /// <summary>Función para obtener la coordenada Y actual.</summary>
        GET_ACTUAL_Y, 
        /// <summary>Función para obtener el tamaño del lienzo.</summary>
        GET_CANVAS_SIZE, 
        /// <summary>Función para obtener el número de colores utilizados.</summary>
        GET_COLOR_COUNT,
        /// <summary>Función para verificar si el pincel tiene un color específico.</summary>
        IS_BRUSH_COLOR, 
        /// <summary>Función para verificar si el pincel tiene un tamaño específico.</summary>
        IS_BRUSH_SIZE, 
        /// <summary>Función para verificar si un punto del lienzo tiene un color específico.</summary>
        IS_CANVAS_COLOR,

        // Control Structures
        /// <summary>Comando para saltar a una etiqueta.</summary>
        GOTO,

        // Operator
        /// <summary>Operador de suma.</summary>
        SUM, 
        /// <summary>Operador de resta.</summary>
        MIN, 
        /// <summary>Operador de multiplicación.</summary>
        MULT, 
        /// <summary>Operador de división.</summary>
        DIV, 
        /// <summary>Operador de módulo.</summary>
        MOD, 
        /// <summary>Operador de potencia.</summary>
        POW,
        /// <summary>Operador de igualdad.</summary>
        EQUAL_EQUAL, 
        /// <summary>Operador de desigualdad.</summary>
        BANG_EQUAL, 
        /// <summary>Operador mayor que.</summary>
        GREATER, 
        /// <summary>Operador mayor o igual que.</summary>
        GREATER_EQUAL, 
        /// <summary>Operador menor que.</summary>
        LESS, 
        /// <summary>Operador menor o igual que.</summary>
        LESS_EQUAL,
        /// <summary>Operador lógico AND.</summary>
        AND, 
        /// <summary>Operador lógico OR.</summary>
        OR,

        // Symbols
        /// <summary>Paréntesis izquierdo.</summary>
        LEFT_PAREN, 
        /// <summary>Paréntesis derecho.</summary>
        RIGHT_PAREN, 
        /// <summary>Corchete izquierdo.</summary>
        LEFT_BRACKET, 
        /// <summary>Corchete derecho.</summary>
        RIGHT_BRACKET, 
        /// <summary>Coma.</summary>
        COMMA,

        // Assignment
        /// <summary>Operador de asignación (flecha).</summary>
        ARROW,

        // Literal
        /// <summary>Identificador (nombre de variable o etiqueta).</summary>
        IDENTIFIER, 
        /// <summary>Valor numérico.</summary>
        NUMBER, 
        /// <summary>Valor literal de color.</summary>
        COLOR_LITERAL,

        // Boolean Literals
        /// <summary>Valor booleano verdadero.</summary>
        TRUE, 
        /// <summary>Valor booleano falso.</summary>
        FALSE,

        // End of file
        /// <summary>Fin de archivo.</summary>
        EOF,

        // Jumpline
        /// <summary>Salto de línea.</summary>
        NEWLINE,
    }
}