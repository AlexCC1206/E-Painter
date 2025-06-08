using System;

namespace EPainter
{
    public class ParseError : Exception
    {
        public ParseError() : base() { }
        public ParseError(string message) : base(message) { }
    }
}