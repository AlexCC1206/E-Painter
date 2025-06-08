using System;
using System.Collections.Generic;

namespace EPainter
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

            // Colors
            {"Red", TokenType.COLOR_LITERAL},
            {"Blue", TokenType.COLOR_LITERAL},
            {"Green", TokenType.COLOR_LITERAL},
            {"Yellow", TokenType.COLOR_LITERAL},
            {"Orange", TokenType.COLOR_LITERAL},
            {"Purple", TokenType.COLOR_LITERAL},
            {"Black", TokenType.COLOR_LITERAL},
            {"White", TokenType.COLOR_LITERAL},
            {"Transparent", TokenType.COLOR_LITERAL}
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
                case ',': AddToken(TokenType.COMMA); break;
                case '[': AddToken(TokenType.LEFT_BRACKET); break;
                case ']': AddToken(TokenType.RIGHT_BRACKET); break;

                // Operators
                case '+': AddToken(TokenType.SUM); break;
                case '-': AddToken(TokenType.MIN); break;
                case '/': AddToken(TokenType.DIV); break;
                case '%': AddToken(TokenType.MOD); break;
                case '*':
                    AddToken(Match('*') ? TokenType.POW : TokenType.MULT);
                    break;

                // Comparison operators
                case '<':
                    if (Match('-')) AddToken(TokenType.LEFT_ARROW);
                    else AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                // Logical operators
                case '&':
                    if (Match('&')) AddToken(TokenType.AND);
                    break;
                case '|':
                    if (Match('|')) AddToken(TokenType.OR);
                    break;

                // Jumpline
                case '\n':
                    AddToken(TokenType.NEWLINE);
                    line++;
                    break;

                // Strings
                case '"': String(); break;

                default:
                    if (char.IsDigit(c))
                    {
                        Number();
                    }
                    else if (char.IsLetter(c) || c == '_')
                    {
                        Identifier();
                    }
                    else if (!char.IsWhiteSpace(c))
                    {
                        throw new ScannerException(line, "Unexpected character: " + c);
                    }
                    break;
            }
        }

        /// <summary>
        /// Identifica un identificador o palabra clave.
        /// </summary>
        private void Identifier()
        {
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_') Advance();

            string text = Source.Substring(start, current - start);

            if (Keywords.TryGetValue(text, out TokenType type))
            {
                if (type == TokenType.COLOR_LITERAL)
                {
                    AddToken(type, text);
                }
                else
                {
                    AddToken(type);
                }
            }
            else
            {
                AddToken(TokenType.IDENTIFIER, text);
            }
        }

        /// <summary>
        /// Escanea un número y lo convierte en un token.
        /// </summary>
        private void Number()
        {
            while (char.IsDigit(Peek())) Advance();

            string numberText = Source.Substring(start, current - start);
            if (int.TryParse(numberText, out int intValue))
            {
                AddToken(TokenType.NUMBER, intValue);
            }
            else
            {
                throw new ScannerException(line, "Invalid number format.");
            }
            
        }

        /// <summary>
        /// Escanea una cadena de texto y la convierte en un token.
        /// </summary>
        private void String()
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

            string value = Source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        /// <summary>
        /// Verifica si el siguiente carácter coincide con el esperado.
        /// </summary>
        /// <param name="expected">El carácter esperado.</param>
        /// <returns>True si coincide, de lo contrario False.</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source[current] != expected) return false;

            current++;
            return true;
        }

        /// <summary>
        /// Obtiene el carácter actual sin avanzar el puntero.
        /// </summary>
        /// <returns>El carácter actual.</returns>
        public char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source[current];
        }

        /// <summary>
        /// Verifica si se ha alcanzado el final del código fuente.
        /// </summary>
        /// <returns>True si se ha alcanzado el final, de lo contrario False.</returns>
        private bool IsAtEnd()
        {
            return current >= Source.Length;
        }

        /// <summary>
        /// Avanza el puntero al siguiente carácter y lo devuelve.
        /// </summary>
        /// <returns>El carácter actual antes de avanzar.</returns>
        private char Advance()
        {
            current++;
            return Source[current - 1];
        }

        /// <summary>
        /// Agrega un token a la lista de tokens con un valor literal.
        /// </summary>
        /// <param name="type">El tipo de token.</param>
        /// <param name="literal">El valor literal del token.</param>
        private void AddToken(TokenType type, object literal = null)
        {
            string text = Source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
