using System;
using System.Collections.Generic;

namespace EPainter
{
    public class ErrorHandler
    {
        public static bool HadError;
        public static bool HadRuntimeError;
        public static List<string> ErrorMessages = new List<string>();

        public static void Reset()
        {
            HadError = false;
            HadRuntimeError = false;
            ErrorMessages.Clear();
        }

        public static void Error(int Line, string message)
        {
            var fullMessage = $"[Line {Line}] Error: {message}";
            ErrorMessages.Add(fullMessage);
            Report(Line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, "at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            if (error.Token != null)
            {
                Console.Error.WriteLine($"[Line {error.Token.Line}] {error.Message}");
            }
            else
            {
                Console.Error.WriteLine(error.Message);
            }
            HadRuntimeError = true;
        }

        private static void Report(int Line, string where, string message)
        {
            Console.Error.WriteLine($"[Line {Line}] Error{where}: {message}");
            HadError = true;
        }
    }

    public class ParseError : Exception { }

    public class EPainterException : Exception
    {
        public int Line { get; }

        public EPainterException(int line, string message) : base($"Line {line}: {message}")
        {
            Line = line;
        }
    }
}