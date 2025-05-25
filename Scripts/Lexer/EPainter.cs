using System;

namespace EPainter
{
    public static class EPainter
    {
        public static bool hadError = false;

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[línea {line}] Error{where}: {message}");
            hadError = true;
        }

        public static void Error(Token token, string message)
        {
            Console.WriteLine($"[Línea {token.line}] Error en '{token.lexeme}': {message}");
        }
    }
}