using System;
using System.Collections.Generic;

namespace EPainter.Core
{
    public static class ErrorReporter
    {
        public static List<string> errors = new List<string>();
        public static List<string> semanticErrors = new List<string>();
        public static List<string> runtimeErrors = new List<string>();
        public static bool HasErrors => errors.Count > 0;
        public static bool HasSemanticErrors => semanticErrors.Count > 0;
        public static bool HasRuntimeErrors => runtimeErrors.Count > 0;

        public static IReadOnlyList<string> Errors => errors.AsReadOnly();
        public static IReadOnlyList<string> SemanticErrors => semanticErrors.AsReadOnly();
        public static IReadOnlyList<string> RuntimeErrors => runtimeErrors.AsReadOnly();

        public static void Reset()
        {
            errors.Clear();
            semanticErrors.Clear();
            runtimeErrors.Clear();
        }

        public static void ReportError(int line, string where, string message)
        {
            string error = $"[line {line}] Error{where}: {message}";
            errors.Add(error);
            Console.Error.WriteLine(error);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                ReportError(token.Line, "at end", message);
            }
            else
            {
                ReportError(token.Line, $"in '{token.Lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            string errorMsg = $"[Línea {error.Token?.Line ?? 0}] Error en tiempo de ejecución: {error.Message}";
            runtimeErrors.Add(errorMsg);
            Console.Error.WriteLine(errorMsg);
        }

        public static void SemanticError(int line, string message)
        {
            string errorMsg = $"[Línea {line}] Error semántico: {message}";
            semanticErrors.Add(errorMsg);
            Console.Error.WriteLine(errorMsg);
        }
        
        public static void ResolutionError(Token name, string message)
        {
            Error(name, message);
        }

        public static void PrintAllErrors()
        {
            foreach (var error in errors)
            {
                Console.Error.WriteLine(error);
            }
            
            foreach (var semanticError in semanticErrors)
            {
                Console.Error.WriteLine(semanticError);
            }

            foreach (var runtimeError in runtimeErrors)
            {
                Console.Error.WriteLine(runtimeError);
            }
        }
    }
}