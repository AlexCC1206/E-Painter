using System;
using System.Collections.Generic;

namespace EPainter
{
    class Scanner
    {
        private string source;
        private List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        // Commands
        {"Spawn", TokenType.SPAWN},
        {"Color", TokenType.COLOR},
        {"Size", TokenType.SIZE},
        {"DrawLine", TokenType.DRAWLINE},
        {"DrawCircle", TokenType.DRAWCIRCLE},
        {"DrawRectangle", TokenType.DRAWRECTANGLE},
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
                ScanTokens();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

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

        private void Number()
        {
            while (char.IsDigit(Peek())) Advance();

            string value = source.Substring(start, current - start);
            AddToken(TokenType.NUMBER, Convert.ToInt32(value));
        }

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

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        public char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
