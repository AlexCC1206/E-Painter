using System;
using System.Collections.Generic;

namespace EPainter.Core
{
    /// <summary>
    /// Clase estática para gestionar y reportar errores en E-Painter.
    /// </summary>
    public static class ErrorReporter
    {
        /// <summary>
        /// Lista de errores de compilación.
        /// </summary>
        public static List<string> errors = new List<string>();
        
        /// <summary>
        /// Lista de errores en tiempo de ejecución.
        /// </summary>
        public static List<string> runtimeErrors = new List<string>();
        
        /// <summary>
        /// Indica si hay errores de compilación.
        /// </summary>
        public static bool HasErrors => errors.Count > 0;
        
        /// <summary>
        /// Indica si hay errores en tiempo de ejecución.
        /// </summary>
        public static bool HasRuntimeErrors => runtimeErrors.Count > 0;

        /// <summary>
        /// Proporciona una lista de solo lectura con todos los errores de compilación.
        /// </summary>
        public static IReadOnlyList<string> Errors => errors.AsReadOnly();
        
        /// <summary>
        /// Proporciona una lista de solo lectura con todos los errores en tiempo de ejecución.
        /// </summary>
        public static IReadOnlyList<string> RuntimeErrors => runtimeErrors.AsReadOnly();

        /// <summary>
        /// Restablece todas las listas de errores, eliminando todos los errores registrados.
        /// </summary>
        public static void Reset()
        {
            errors.Clear();
            runtimeErrors.Clear();
        }

        /// <summary>
        /// Reporta un error de compilación.
        /// </summary>
        /// <param name="line">La línea donde ocurrió el error.</param>
        /// <param name="where">Contexto adicional sobre la ubicación del error.</param>
        /// <param name="message">El mensaje de error.</param>
        public static void ReportError(int line, string where, string message)
        {
            string error = $"[line {line}] Error{where}: {message}";
            errors.Add(error);
            Console.Error.WriteLine(error);
        }

        /// <summary>
        /// Reporta un error asociado con un token específico.
        /// </summary>
        /// <param name="token">El token donde ocurrió el error.</param>
        /// <param name="message">El mensaje de error.</param>
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

        /// <summary>
        /// Reporta un error en tiempo de ejecución.
        /// </summary>
        /// <param name="error">El objeto de error en tiempo de ejecución.</param>
        public static void RuntimeError(RuntimeError error)
        {
            string errorMsg = $"[Line {error.Token?.Line ?? 0}] Runtime error: {error.Message}";
            runtimeErrors.Add(errorMsg);
            Console.Error.WriteLine(errorMsg);
        }

        /// <summary>
        /// Reporta un error de resolución asociado con un nombre específico.
        /// </summary>
        /// <param name="name">El token que representa el nombre con el problema.</param>
        /// <param name="message">El mensaje de error.</param>
        public static void ResolutionError(Token name, string message)
        {
            Error(name, message);
        }

        /// <summary>
        /// Imprime todos los errores registrados en la consola.
        /// </summary>
        public static void PrintAllErrors()
        {
            foreach (var error in errors)
            {
                Console.Error.WriteLine(error);
            }

            foreach (var runtimeError in runtimeErrors)
            {
                Console.Error.WriteLine(runtimeError);
            }
        }
    }
}