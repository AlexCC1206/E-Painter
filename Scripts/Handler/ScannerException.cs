using System;

namespace EPainter
{
    public class ScannerException : Exception
    {
        public int Line { get; }

        public ScannerException(int line, string message) : base(message)
        {
            Line = line;
        }

        public override string ToString()
        {
            return $"[Line {Line}] Scanner Error: {Message}";
        }
    }
}