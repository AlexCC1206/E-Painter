using System;

namespace EPainter.Core
{
    public class ErrorReport
    {
        private static bool hadError = false;

        public static void ReportError(Token token, String message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " al final", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}