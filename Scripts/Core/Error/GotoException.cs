using System;

namespace EPainter.Core
{
    public class GotoException : Exception
    {
        public string Label { get; }

        public GotoException(string label) : base("Goto instruction")
        {
            Label = label;
        }
    }
}