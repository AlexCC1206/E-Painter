using System;

namespace EPainter.Core
{
    public class ParseError : Exception
    {
        public ParseError() : base() { }
        public ParseError(string message) : base(message) { }
    }
}