using System.Collections.Generic;

namespace EPainter.Core
{
    /// <summary>
    /// Analizador léxico que convierte código fuente en una lista de Tokens.
    /// </summary>
    public class Scanner
    {
        #region Campos
        /// <summary>
        /// El código fuente a analizar.
        /// </summary>
        private string Source;

        /// <summary>
        /// Lista de Tokens encontrados durante el análisis.
        /// </summary>
        private List<Token> Tokens = new List<Token>();

        /// <summary>
        /// Índice de inicio del token actual.
        /// </summary>
        private int Start = 0;

        /// <summary>
        /// Índice actual en el código fuente.
        /// </summary>
        private int Current = 0;

        /// <summary>
        /// Número de línea actual en el código fuente.
        /// </summary>
        private int Line = 1;

        /// <summary>
        /// Conjunto de colores válidos permitidos en el lenguaje.
        /// </summary>
        private static readonly HashSet<string> ValidColors = new()
        {
            "Red", "Blue", "Green", "Yellow", "Orange",
            "Purple", "Black", "White", "Transparent"
        };

        /// <summary>
        /// Diccionario que mapea palabras clave a sus tipos de Tokens correspondientes.
        /// </summary>
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"Spawn", TokenType.SPAWN},
            {"Color", TokenType.COLOR},
            {"Size", TokenType.SIZE},
            {"DrawLine", TokenType.DRAW_LINE},
            {"DrawCircle", TokenType.DRAW_CIRCLE},
            {"DrawRectangle", TokenType.DRAW_RECTANGLE },
            {"Fill", TokenType.FILL},

            {"GetActualX", TokenType.GET_ACTUAL_X},
            {"GetActualY", TokenType.GET_ACTUAL_Y},
            {"GetCanvasSize", TokenType.GET_CANVAS_SIZE},
            {"GetColorCount", TokenType.GET_COLOR_COUNT},
            {"IsBrushColor", TokenType.IS_BRUSH_COLOR},
            {"IsBrushSize", TokenType.IS_BRUSH_SIZE},
            {"IsCanvasColor", TokenType.IS_CANVAS_COLOR},

            {"GoTo", TokenType.GOTO},

            {"True", TokenType.TRUE},
            {"False", TokenType.FALSE}
        };
        #endregion

        #region Initialización
        /// <summary>
        /// Inicializa una nueva instancia de la clase Scanner.
        /// </summary>
        /// <param name="source">El código fuente a analizar.</param>
        public Scanner(string source)
        {
            Source = source;
        }
        #endregion

        #region Análisis Léxico Principal
        /// <summary>
        /// Escanea todos los Tokens del código fuente.
        /// </summary>
        /// <remarks>
        /// Esta implementación permite detectar múltiples errores durante el análisis léxico.
        /// Cuando encuentra un error, lo reporta y continúa el análisis en lugar de detenerse.
        /// </remarks>
        /// <returns>Lista de Tokens encontrados.</returns>
        public List<Token> scanTokens()
        {
            while (!IsAtEnd())
            {
                Start = Current;
                ScanTokens();

            }

            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        /// <summary>
        /// Analiza y añade el siguiente token del código fuente.
        /// </summary>
        private void ScanTokens()
        {
            char c = Advance();
            switch (c)
            {
                // Caracteres de un solo token
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '[': AddToken(TokenType.LEFT_BRACKET); break;
                case ']': AddToken(TokenType.RIGHT_BRACKET); break;
                case ',': AddToken(TokenType.COMMA); break;

                // Operadores aritméticos
                case '+': AddToken(TokenType.SUM); break;
                case '-': AddToken(TokenType.MIN); break;
                case '*':
                    if (Match('*')) AddToken(TokenType.POW);
                    else AddToken(TokenType.MULT);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // Comentario de línea 
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    }
                    else
                    {
                        AddToken(TokenType.DIV);
                    }
                    break;
                case '%': AddToken(TokenType.MOD); break;

                // Operadores de comparación
                case '=':
                    if (Match('=')) AddToken(TokenType.EQUAL_EQUAL);
                    break;
                case '!':
                    if (Match('=')) AddToken(TokenType.BANG_EQUAL);
                    break;
                case '<':
                    if (Match('-')) AddToken(TokenType.ARROW);
                    else if (Match('=')) AddToken(TokenType.LESS_EQUAL);
                    else AddToken(TokenType.LESS);
                    break;
                case '>':
                    if (Match('=')) AddToken(TokenType.GREATER_EQUAL);
                    else AddToken(TokenType.GREATER);
                    break;


                case '&':
                    if (Match('&')) AddToken(TokenType.AND);
                    break;
                case '|':
                    if (Match('|')) AddToken(TokenType.OR);
                    break;


                case '"': ColorLiteral(); break;


                case '\n':
                    Line++;
                    AddToken(TokenType.NEWLINE);
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        ErrorReport.Error(Line, $"Unexpected character '{c}'");
                    }
                    break;
            }
        }
        #endregion

        #region Reconocimiento de Tokens Específicos
        /// <summary>
        /// Analiza un identificador en el código fuente.
        /// </summary>
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source.Substring(Start, Current - Start);

            if (Keywords.TryGetValue(text, out TokenType type))
            {
                AddToken(type);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER);
            }
        }

        /// <summary>
        /// Analiza un literal numérico en el código fuente.
        /// </summary>
        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            AddToken(TokenType.NUMBER, int.Parse(Source.Substring(Start, Current - Start)));
        }

        /// <summary>
        /// Analiza un literal de color entre comillas dobles en el código fuente.
        /// </summary>
        private void ColorLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    Line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                ErrorReport.Error(Line, "Undeterminated color literal");
                return;
            }

            Advance();

            string color = Source.Substring(Start + 1, Current - Start - 2);
            if (!ValidColors.Contains(color))
            {
                ErrorReport.Error(Line, $"Invalid color: '{color}'");
                return;
            }

            AddToken(TokenType.COLOR_LITERAL, color);
        }
        #endregion

        #region Utilidades del Scanner
        /// <summary>
        /// Verifica si el caracter actual coincide con el esperado y avanza si es así.
        /// </summary>
        /// <param name="expected">El caracter esperado.</param>
        /// <returns>True si hay coincidencia, false en caso contrario.</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source[Current] != expected) return false;

            Current++;
            return true;
        }

        /// <summary>
        /// Observa el caracter actual sin avanzar.
        /// </summary>
        /// <returns>El caracter actual o '\0' si se llegó al final del código fuente.</returns>
        public char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source[Current];
        }

        /// <summary>
        /// Verifica si se ha llegado al final del código fuente.
        /// </summary>
        /// <returns>True si se ha llegado al final del código fuente, false en caso contrario.</returns>
        private bool IsAtEnd()
        {
            return Current >= Source.Length;
        }

        /// <summary>
        /// Avanza al siguiente caracter y devuelve el actual.
        /// </summary>
        /// <returns>El caracter actual antes de avanzar.</returns>
        private char Advance()
        {
            Current++;
            return Source[Current - 1];
        }

        /// <summary>
        /// Verifica si un caracter es un dígito.
        /// </summary>
        /// <param name="c">El caracter a verificar.</param>
        /// <returns>True si es un dígito, false en caso contrario.</returns>
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Verifica si un caracter es una letra.
        /// </summary>
        /// <param name="c">El caracter a verificar.</param>
        /// <returns>True si es una letra, false en caso contrario.</returns>
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// Verifica si un caracter es alfanumérico o un guión bajo.
        /// </summary>
        /// <param name="c">El caracter a verificar.</param>
        /// <returns>True si es alfanumérico o guión bajo, false en caso contrario.</returns>
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c) || c == '_';
        }

        /// <summary>
        /// Añade un token a la lista de Tokens.
        /// </summary>
        /// <param name="type">El tipo de token.</param>
        /// <param name="literal">El valor literal del token, opcional.</param>
        private void AddToken(TokenType type, object literal = null)
        {
            string text = Source.Substring(Start, Current - Start);
            Tokens.Add(new Token(type, text, literal, Line));
        }
        #endregion
    }
}
