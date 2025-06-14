using System;
using System.Collections.Generic;
using System.Linq;

namespace EPainter.Core
{
    /// <summary>
    /// Clase encargada de escanear el código fuente y generar una lista de tokens.
    /// </summary>
    public class Scanner
    {
        private string Source;
        private List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        /// <summary>
        /// Diccionario que contiene las palabras clave y su tipo de token correspondiente.
        /// </summary>
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            // Commands
            {"Spawn", TokenType.SPAWN},
            {"Color", TokenType.COLOR},
            {"Size", TokenType.SIZE},
            {"DrawLine", TokenType.DRAW_LINE},
            {"DrawCircle", TokenType.DRAW_CIRCLE},
            {"DrawRectangle", TokenType.DRAW_RECTANGLE },
            {"Fill", TokenType.FILL},

            // Function
            {"GetActualX", TokenType.GET_ACTUAL_X},
            {"GetActualY", TokenType.GET_ACTUAL_Y},
            {"GetCanvasSize", TokenType.GET_CANVAS_SIZE},
            {"GetColorCount", TokenType.GET_COLOR_COUNT},
            {"IsBrushColor", TokenType.IS_BRUSH_COLOR},
            {"IsBrushSize", TokenType.IS_BRUSH_SIZE},
            {"IsCanvasColor", TokenType.IS_CANVAS_COLOR},

            // Control
            {"GoTo", TokenType.GOTO},

            // Booleans
            {"True", TokenType.TRUE},
            {"False", TokenType.FALSE}
        };

        /// <summary>
        /// Constructor de la clase Scanner.
        /// </summary>
        /// <param name="source">El código fuente a escanear.</param>
        public Scanner(string source)
        {
            Source = source;
        }

        /// <summary>
        /// Escanea el código fuente y genera una lista de tokens.
        /// </summary>
        /// <returns>Lista de tokens reconocidos</returns>
        /// <exception cref="PixelWallEException">Cuando se encuentra un error léxico</exception>
        public List<Token> scanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanTokens();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        /// <summary>
        /// Escanea un carácter y lo convierte en un token.
        /// </summary>
        private void ScanTokens()
        {
            char c = Advance();
            switch (c)
            {
                // Character
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '[': AddToken(TokenType.LEFT_BRACKET); break;
                case ']': AddToken(TokenType.RIGHT_BRACKET); break;
                case ',': AddToken(TokenType.COMMA); break;

                // Operators
                case '+': AddToken(TokenType.SUM); break;
                case '-': AddToken(TokenType.MIN); break;
                case '*':
                    if (Match('*')) AddToken(TokenType.POW);
                    else AddToken(TokenType.MULT);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // Comentario de una línea
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    }
                    else
                    {
                        AddToken(TokenType.DIV);
                    }
                    break;
                case '%': AddToken(TokenType.MOD); break;


                // Comparison operators
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

                // Logical operators
                case '&':
                    if (Match('&')) AddToken(TokenType.AND);
                    break;
                case '|':
                    if (Match('|')) AddToken(TokenType.OR);
                    break;

                // Color
                case '"': ColorLiteral(); break;

                // Jumpline
                case '\n':
                    line++;
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
                        throw new ScannerException(line, $"Unexpected character: '{c}'");
                    }
                    break;
            }
        }


        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source.Substring(start, current - start);

            if (Keywords.TryGetValue(text, out TokenType type))
            {
                AddToken(type);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER);
            }
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            AddToken(TokenType.NUMBER, double.Parse(Source.Substring(start, current - start)));

        }


        private void ColorLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                throw new ScannerException(line, "Undeterminated string");
            }

            Advance();

            string color = Source.Substring(start + 1, current - start - 2);
            if (!ValidColors.Contains(color))
            {
                throw new ScannerException(line, $"Color no válido: '{color}'");
            }

            AddToken(TokenType.COLOR_LITERAL, color);
        }

        private static readonly HashSet<string> ValidColors = new()
        {
            "Red", "Blue", "Green", "Yellow", "Orange",
            "Purple", "Black", "White", "Transparent"
        };



        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source[current] != expected) return false;

            current++;
            return true;
        }

        public char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source[current];
        }


        private bool IsAtEnd()
        {
            return current >= Source.Length;
        }


        private char Advance()
        {
            current++;
            return Source[current - 1];
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }


        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);

        }

        private void AddToken(TokenType type, object literal = null)
        {
            string text = Source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
