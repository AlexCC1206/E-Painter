public class Token
{
    public TokenType Type;
    public string Value;
    public int LineNumber;

    public Token(TokenType type, string value, int lineNumber)
    {
        Type = type;
        Value = value;
        LineNumber = lineNumber;
    }

    public enum TokenType
    {
        Spawn,
        Color,
        Assign,
        Goto,
        Identifier,
        Number,
        String,
    }

}