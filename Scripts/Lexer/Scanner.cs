using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

class Scanner
{
    private string source;
    private List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source)
    {
        this.source = source;
    }

    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        {"Spawn", TokenType.SPAWN},
        {"Color", TokenType.COLOR},
        {"Size", TokenType.SIZE},
        {"Drawline", TokenType.DRAWLINE},
        {"DrawCircle", TokenType.DRAWCIRCLE},
        {"DrawRectangle", TokenType.DRAWRECTANGLE},
        {"Fill", TokenType.FILL},
        {"GetActualX", TokenType.GETACTUALX},
        {"GetActualY", TokenType.GETACTUALY},
        {"GetCanvasSize", TokenType.GETCANVASIZE},
        {"GetColorCount", TokenType.GETCOLORCOUNT},
        {"IsBrushColor", TokenType.ISBRUSHCOLOR},
        {"IsBrushSize", TokenType.ISBRUSHSIZE},
        {"IsCanvasColor", TokenType.ISCANVASCOLOR},
        {"Goto", TokenType.GOTO},
        {"label", TokenType.LABEL},
        {"var", TokenType.VAR}
    };

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
            case '(': addToken(TokenType.LEFT_PAREN); break;
            case ')': addToken(TokenType.RIGHT_PAREN); break;
            case ',': addToken(TokenType.COMMA); break;
            case ';': addToken(TokenType.SEMICOLON); break;
            case '+': addToken(TokenType.SUM); break;
            case '-': addToken(TokenType.MIN); break;
            case '/': addToken(TokenType.DIV); break;
            case '%': addToken(TokenType.MOD); break;
            
            case '*': 
                addToken(match('*') ? TokenType.POW : TokenType.MULT); 
                break;
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

            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                line++;
                break;

            case '"': String(); break;

            default:
                if(isDigit(c))
                {
                    number();
                }
                else if(isAlpha(c))
                {
                    identifier();
                }
                else
                {
                    ReportError(line, "Unexpected character");
                }
                break;
        }
    }

    private void identifier()
    {
        while(isAlphaNumeric(peek())) Advance();

        string text = source.Substring(start, current - start);
        TokenType type;
        
        if(!keywords.TryGetValue(text, out type))
        {
            type = TokenType.IDENTIFIER;
        }

        addToken(type);
    }

    private void number()
    {
        while(isDigit(peek())) Advance();

        AddToken(TokenType.NUMBER, int.Parse(source.Substring(start, current)));
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
            ReportError(line, "Undeterminated string");
            return;
        }

        Advance();

        string value = source.Substring(start + 1, current - 1);
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

    private bool isAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '-';
    }

    private bool isAlphaNumeric(char c)
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

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
        string text = source.Substring(start, current);
        tokens.Add(new Token(type, text, literal, line));
    }

    private void ReportError(int line, string message)
    {
        throw new Exception($"Error en línea {line}: {message}");
    }
}