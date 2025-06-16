using System;
using System.Collections.Generic;
using System.Linq;

namespace EPainter.Core
{

    public class Scanner
    {
        private string Source;
        private List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;


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


        public Scanner(string source)
        {
            Source = source;
        }


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

                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '[': AddToken(TokenType.LEFT_BRACKET); break;
                case ']': AddToken(TokenType.RIGHT_BRACKET); break;
                case ',': AddToken(TokenType.COMMA); break;


                case '+': AddToken(TokenType.SUM); break;
                case '-': AddToken(TokenType.MIN); break;
                case '*':
                    if (Match('*')) AddToken(TokenType.POW);
                    else AddToken(TokenType.MULT);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    }
                    else
                    {
                        AddToken(TokenType.DIV);
                    }
                    break;
                case '%': AddToken(TokenType.MOD); break;



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
            if (Source[start] == '_')
            {
                throw new ScannerException(line, "Los identificadores no pueden comenzar con guión bajo (_)");
            }
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

            AddToken(TokenType.NUMBER, int.Parse(Source.Substring(start, current - start)));

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
                    (c >= 'A' && c <= 'Z')
                    ;
        }


        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c) || c == '_';

        }

        private void AddToken(TokenType type, object literal = null)
        {
            string text = Source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
