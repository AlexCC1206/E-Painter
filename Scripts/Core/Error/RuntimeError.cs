using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que ocurre durante la ejecución del programa E-Painter.
    /// </summary>
    public class RuntimeError : Exception
    {
        /// <summary>
        /// Obtiene el token asociado con este error en tiempo de ejecución.
        /// </summary>
        public Token Token { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase RuntimeError con un token y mensaje específicos.
        /// </summary>
        /// <param name="token">El token donde ocurrió el error.</param>
        /// <param name="message">El mensaje descriptivo del error.</param>
        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase RuntimeError con un mensaje específico pero sin token asociado.
        /// </summary>
        /// <param name="message">El mensaje descriptivo del error.</param>
        public RuntimeError(string message) : base(message)
        {
            Token = null;
        }
    }
}