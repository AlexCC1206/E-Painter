using System;
using System.Collections.Generic;

namespace EPainter.Core
{
    /// <summary>
    /// Analizador léxico que convierte código fuente en una lista de tokens.
    /// </summary>
    public class Scanner
    {
        #region Fields and Properties
        /// <summary>
        /// El código fuente a analizar.
        /// </summary>
        private string Source;

        /// <summary>
        /// Lista de tokens encontrados durante el análisis.
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

        private readonly List<string> Errors = new();
        public IEnumerable<string> GetErrors() => Errors;

        /// <summary>
        /// Diccionario que mapea palabras clave a sus tipos de tokens correspondientes.
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

        /// <summary>
        /// Conjunto de colores válidos permitidos en el lenguaje.
        /// </summary>
        private static readonly HashSet<string> ValidColors = new()
        {
            "Red", "Blue", "Green", "Yellow", "Orange",
            "Purple", "Black", "White", "Transparent"
        };
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de la clase Scanner.
        /// </summary>
        /// <param name="source">El código fuente a analizar.</param>
        public Scanner(string source)
        {
            Source = source;
        }
        #endregion

        #region Scanning Methods
        /// <summary>
        /// Escanea todos los tokens del código fuente.
        /// </summary>
        /// <remarks>
        /// Esta implementación permite detectar múltiples errores durante el análisis léxico.
        /// Cuando encuentra un error, lo reporta y continúa el análisis en lugar de detenerse.
        /// </remarks>
        /// <returns>Lista de tokens encontrados.</returns>
        public List<Token> scanTokens()
        {
            while (!IsAtEnd())
            {

                Start = Current;
                ScanTokens();
                /*
                try
                {
                    ScanTokens();
                }
                catch (Exception ex)
                {
                    Error(Line, $"Unexpected error: {ex.Message}");
                    Synchronize();
                } */

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
                    else
                    {
                        Error(Line, "Expected '=' after '='.");
                    }
                    break;
                case '!':
                    if (Match('=')) AddToken(TokenType.BANG_EQUAL);
                    else
                    {
                        Error(Line, "Expected '=' after '!'.");
                    }
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
                    else
                    {
                        Error(Line, "Expected '&' after '&'.");
                    }
                    break;
                case '|':
                    if (Match('|')) AddToken(TokenType.OR);
                    else
                    {
                        Error(Line, "Expected '|' after '|'.");
                    }
                    break;


                case '"':
                    ColorLiteral();
                    break;


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
                        Error(Line, $"Unexpected character '{c}'.");
                        /*
                        Error(Line, $"Unexpected character", c);
                        Synchronize();*/
                    }
                    break;
            }
        }
        #endregion

        #region Token Recognition
        /// <summary>
        /// Avanza al siguiente caracter y devuelve el actual.
        /// </summary>
        /// <returns>El caracter actual antes de avanzar.</returns>
        private char Advance()
        {
            /*
            Current++;
            return Source[Current - 1];*/
            return Source[Current++];
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
                    (c >= 'A' && c <= 'Z') || c == '_';
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
        /// Analiza un identificador en el código fuente.
        /// </summary>
        private void Identifier()
        {
            /*
            if (Source[Start] == '_')
            {
                Error(Line, "Identifiers cannot start with underscore (_)");
                Synchronize();
                return;
            }*/
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

            string numberText = Source.Substring(Start, Current - Start);
            //int value = int.Parse(numberText);
            if (!int.TryParse(numberText, out int value))
            {
                Error(Line, $"Invalid number literal: '{numberText}'.");
                return;
            }
            AddToken(TokenType.NUMBER, value);
        }

        /// <summary>
        /// Analiza un literal de color entre comillas dobles en el código fuente.
        /// </summary>
        private void ColorLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                /*
                if (Peek() == '\n')
                {
                    Line++;
                }*/
                Advance();
            }

            if (IsAtEnd())
            {
                Error(Line, "Undeterminated color literal");
                return;
            }

            Advance();

            string value = Source.Substring(Start + 1, Current - Start - 2);
            if (!ValidColors.Contains(value))
            {
                Error(Line, $"Invalid color: '{value}'");
                return;
            }

            AddToken(TokenType.COLOR_LITERAL, value);
        }
        #endregion

        #region Error Handling
        private void Error(int line, string message)
        {
            Errors.Add($"[line {line}] Error: {message}");
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Verifica si se ha llegado al final del código fuente.
        /// </summary>
        /// <returns>True si se ha llegado al final del código fuente, false en caso contrario.</returns>
        private bool IsAtEnd()
        {
            return Current >= Source.Length;
        }

        /// <summary>
        /// Añade un token a la lista de tokens.
        /// </summary>
        /// <param name="type">El tipo de token.</param>
        /// <param name="literal">El valor literal del token, opcional.</param>
        private void AddToken(TokenType type, object literal = null)
        {
            string text = Source.Substring(Start, Current - Start);
            Tokens.Add(new Token(type, text, literal, Line));
        }
        #endregion
        /*
                /// <summary>
                /// Reporta un error del scanner pero no lanza una excepción, permitiendo que el análisis continúe.
                /// </summary>
                /// <param name="line">Línea donde ocurrió el error.</param>
                /// <param name="message">Mensaje descriptivo del error.</param>
                /// <param name="character">Carácter problemático (opcional).</param>
                private void Error(int line, string message, char? character = null)
                {
                    ErrorReporter.ReportScannerError(line, message, character);
                }

                /// <summary>
                /// Método de sincronización para recuperarse de un error y continuar el análisis.
                /// </summary>
                private void Synchronize()
                {
                    Advance();

                    while (!IsAtEnd())
                    {
                        if (Peek() == '\n') return;

                        switch (Peek())
                        {
                            case '(':
                            case ')':
                            case '[':
                            case ']':
                            case ',':
                            case '+':
                            case '-':
                            case '*':
                            case '/':
                            case '"':
                                return;
                        }

                        Advance();
                    }
                }
        */
    }
}
