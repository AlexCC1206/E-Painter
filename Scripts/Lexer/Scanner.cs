using System;
using System.Collections.Generic;
using EPainter;

namespace EPainter
{
    class Scanner
{
    private string source;
    private List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;

    private static readonly Dictionary<string, TokenType> keywords = new()
    {

        // Commands
        {"Spawn", TokenType.SPAWN},
        {"Color", TokenType.COLOR},
        {"Size", TokenType.SIZE},
        {"Drawline", TokenType.DRAWLINE},
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

    public List<Token> scanTokens()
    {
        while(!isAtEnd())
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
            case '(': addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case ',': addToken(TokenType.COMMA); break;

            // Operators
            case '+': addToken(TokenType.SUM); break;
            case '-': addToken(TokenType.MIN); break;
            case '/': addToken(TokenType.DIV); break;
            case '%': addToken(TokenType.MOD); break;
            case '*': 
                addToken(match('*') ? TokenType.POW : TokenType.MULT); 
                break;

            // Comparison operators
            case '&':
                addToken(match('&') ? TokenType.AND : TokenType.AND);
                break;
            case '|':
                addToken(match('|') ? TokenType.OR : TokenType.OR);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;

            // Ignore whitespace
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                line++;
                break;

            // Strings
            case '"': String(); break;

            default:
                if(char.IsDigit(c))
                {
                    number();
                }
                else if(char.IsLetter(c) || c == '_')
                {
                    identifier();
                }
                else
                {
                    EPainter.Error(line, "Unexpected character" + c);
                }
                break;
        }
    }

    private void identifier()
    {
        while(char.IsLetterOrDigit(peek()) || peek() == '_') Advance();

        string text = source.Substring(start, current - start);
        
        if(!keywords.TryGetValue(text, out TokenType type))
        {
            addToken(type);
        }
        else
        {
            addToken(TokenType.IDENTIFIER);
        }
    }

    private void number()
    {
        while(char.IsDigit(peek())) Advance();

        if (peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance();
            while (char.IsDigit(peek())) Advance();
        }

        AddToken(TokenType.NUMBER, 
            double.Parse(source.Substring(start, current - start)));
    }

        

        private void String()
        {
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n')
                {
                    line++;
                }
                Advance();
            }

            if (isAtEnd())
            {
                EPainter.Error(line, "Undeterminated string");
                return;
            }

            Advance();

            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        private bool match(char expected)
        {
            if(isAtEnd()) return false;
            if(source[current] != expected) return false;

            current++;
            return true;
        }

        public char peek()
        {
            if(isAtEnd()) return '\0';

            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';

            return source[current + 1];
        }
/*
    private bool isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }*/

    private bool isAtEnd()
    {
        return current >= source.Length;
    }

    private char Advance()
    {
        current++;
        return source[current - 1];
    }

    private void addToken(TokenType type)
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
