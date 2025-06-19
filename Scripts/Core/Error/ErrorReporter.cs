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
        private static List<string> compilationErrors = new List<string>();
        
        /// <summary>
        /// Lista de errores en tiempo de ejecución.
        /// </summary>
        private static List<string> runtimeErrors = new List<string>();
        
        /// <summary>
        /// Lista de errores específicos del scanner.
        /// </summary>
        private static List<string> scannerErrors = new List<string>();
        
        /// <summary>
        /// Indica si hay errores de compilación.
        /// </summary>
        public static bool HasCompilationErrors => compilationErrors.Count > 0;
        
        /// <summary>
        /// Indica si hay errores en tiempo de ejecución.
        /// </summary>
        public static bool HasRuntimeErrors => runtimeErrors.Count > 0;

        /// <summary>
        /// Indica si hay errores específicos del scanner.
        /// </summary>
        public static bool HasScannerErrors => scannerErrors.Count > 0;

        /// <summary>
        /// Proporciona una lista de solo lectura con todos los errores de compilación.
        /// </summary>
        public static IReadOnlyList<string> CompilationErrors => compilationErrors.AsReadOnly();
        
        /// <summary>
        /// Proporciona una lista de solo lectura con todos los errores en tiempo de ejecución.
        /// </summary>
        public static IReadOnlyList<string> RuntimeErrors => runtimeErrors.AsReadOnly();

        /// <summary>
        /// Proporciona una lista de solo lectura con todos los errores del scanner.
        /// </summary>
        public static IReadOnlyList<string> ScannerErrors => scannerErrors.AsReadOnly();

        /// <summary>
        /// Restablece todas las listas de errores, eliminando todos los errores registrados.
        /// </summary>
        public static void Reset()
        {
            compilationErrors.Clear();
            runtimeErrors.Clear();
            scannerErrors.Clear();
        }

        /// <summary>
        /// Reporta un error de compilación.
        /// </summary>
        /// <param name="error">El error a reportar.</param>
        public static void ReportCompilationError(string error)
        {
            compilationErrors.Add(error);
            Console.Error.WriteLine(error);
        } 

        /// <summary>
        /// Reporta un error asociado con un token específico.
        /// </summary>
        /// <param name="token">El token donde ocurrió el error.</param>
        /// <param name="message">El mensaje de error.</param>
        public static void ReportTokenError(Token token, string message)
        {
            string error;
            if (token.Type == TokenType.EOF)
            {
                error = $"[Line {token.Line}] Error at end: {message}";
            }
            else
            {
                error = $"[Line {token.Line}] Error in '{token.Lexeme}': {message}";
            }

            ReportCompilationError(error);
        }

        /// <summary>
        /// Reporta un error específico del scanner.
        /// </summary>
        /// <param name="line">Línea donde ocurrió el error.</param>
        /// <param name="message">Mensaje descriptivo del error.</param>
        /// <param name="character">Carácter problemático (opcional).</param>
        public static void ReportScannerError(int line, string message, char? character = null)
        {
            string error;
            if (character.HasValue)
            {
                error = $"[Line {line}] Error at '{character}': {message}";
            }
            else
            {
                error = $"[Line {line}] Error: {message}";
            }
            
            scannerErrors.Add(error);
            ReportCompilationError(error);
        }

        /// <summary>
        /// Reporta un error en tiempo de ejecución.
        /// </summary>
        /// <param name="error">El objeto de error en tiempo de ejecución.</param>
        public static void RuntimeError(EPainterException error)
        {
            string errorMsg = error.ToString();
            runtimeErrors.Add(errorMsg);
            Console.Error.WriteLine(errorMsg);
        }
    }
}