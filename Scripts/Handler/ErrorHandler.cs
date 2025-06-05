using System;
using System.Collections.Generic;

namespace EPainter
{
    public class ErrorHandler
    {
        public static int ErrorCount { get; private set; } = 0;
        public static bool HadError => ErrorCount > 0;

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Console.Error.WriteLine($"[line {token.Line}] Error at end: {message}");
            }
            else
            {
                Console.Error.WriteLine($"[line {token.Line}] Error at '{token.Lexeme}': {message}");
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"[Line {error.Token.Line}] {error.Message}");
        }

        public static void ResetErrorState()
        {
            ErrorCount = 0;
        }
    }

    public class EPainterException : Exception
    {
        public int Line { get; }

        public EPainterException(int line, string message) : base($"Line {line}: {message}")
        {
            Line = line;
        }
    }
}