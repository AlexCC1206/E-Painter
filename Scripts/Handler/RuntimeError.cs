using System;

namespace EPainter
{
    public class RuntimeError : Exception
    {
        public Token Token;

        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}