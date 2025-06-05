using System;

namespace EPainter
{
    public static class ErrorReporter
    {
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        }
    }
}