using System;
using System.Collections.Generic;

namespace EPainter
{
    /// <summary>
    /// Clase encargada de escanear el código fuente y generar una lista de tokens.
    /// </summary>
    public class Scanner
    {
        private string source;
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
        {"GetActualX", TokenType.GETACTUALX},
        {"GetActualY", TokenType.GETACTUALY},
        {"GetCanvasSize", TokenType.GETCANVASIZE},
        {"GetColorCount", TokenType.GETCOLORCOUNT},
        {"IsBrushColor", TokenType.ISBRUSHCOLOR},
        {"IsBrushSize", TokenType.ISBRUSHSIZE},
        {"IsCanvasColor", TokenType.ISCANVASCOLOR},

        // Control
        {"GoTo", TokenType.GOTO},

        // Colors
        {"Red", TokenType.RED},
        {"Blue", TokenType.BLUE},
        {"Green", TokenType.GREEN},
        {"Yellow", TokenType.YELLOW},
        {"Orange", TokenType.ORANGE},
        {"Purple", TokenType.PURPLE},
        {"Black", TokenType.BLACK},
        {"White", TokenType.WHITE},
        {"Transparent", TokenType.TRANSPARENT}
        };

        /// <summary>
        /// Constructor de la clase Scanner.
        /// </summary>
        /// <param name="source">El código fuente a escanear.</param>
        public Scanner(string source)
        {
            this.source = source;
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
                try
                {
                    ScanTokens();
                }
                catch (EPainterException)
                {
                    while (!IsAtEnd() && !char.IsWhiteSpace(Peek()))
                    {
                        Advance();
                    }
                }
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
                case '/': AddToken(TokenType.DIV); break;
                case '%': AddToken(TokenType.MOD); break;
                case '*':
                    AddToken(Match('*') ? TokenType.POW : TokenType.MULT);
                    break;

                // Comparison operators
                case '&':
                    if (Match('&')) AddToken(TokenType.AND);
                    break;
                case '|':
                    if (Match('|')) AddToken(TokenType.OR);
                    break;
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
                        ErrorHandler.Error(line, "Unexpected character" + c);
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

            string text = source.Substring(start, current - start);

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
        /// Escanea un número y lo convierte en un token.
        /// </summary>
        private void Number()
        {
            while (char.IsDigit(Peek())) Advance();

            string value = source.Substring(start, current - start);
            AddToken(TokenType.NUMBER, Convert.ToInt32(value));
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
                ErrorHandler.Error(line, "Undeterminated string");
                return;
            }

            Advance();

            string value = source.Substring(start + 1, current - start - 2);
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
            if (source[current] != expected) return false;

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
            return source[current];
        }

        /// <summary>
        /// Verifica si se ha alcanzado el final del código fuente.
        /// </summary>
        /// <returns>True si se ha alcanzado el final, de lo contrario False.</returns>
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        /// <summary>
        /// Avanza el puntero al siguiente carácter y lo devuelve.
        /// </summary>
        /// <returns>El carácter actual antes de avanzar.</returns>
        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        /// <summary>
        /// Agrega un token a la lista de tokens.
        /// </summary>
        /// <param name="type">El tipo de token.</param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Agrega un token a la lista de tokens con un valor literal.
        /// </summary>
        /// <param name="type">El tipo de token.</param>
        /// <param name="literal">El valor literal del token.</param>
        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
