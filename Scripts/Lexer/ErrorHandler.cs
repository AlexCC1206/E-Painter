using System;
using System.Collections.Generic;

namespace EPainter
{
    public class ErrorHandler
    {
        public static bool HadError;
        public static bool HadRuntimeError;
        public static List<string> ErrorMessages;

        public static void Reset()
        {
            HadError = false;
            HadRuntimeError = false;
            ErrorMessages.Clear();
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                Report(token.line, "at end", message);
            }
            else
            {
                Report(token.line, $" at '{token.lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            if (error.Token != null)
            {
                Console.Error.WriteLine($"[line {error.Token.line}] {error.Message}");
            }
            else
            {
                Console.Error.WriteLine(error.Message);
            }
            HadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            HadError = true;
        }
    }

    public class ParseError : Exception{}

    public class RuntimeError : Exception
    {
        public Token Token;

        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }

        public RuntimeError(string message) : base(message)
        {
            Token = null;
        }

    }
}