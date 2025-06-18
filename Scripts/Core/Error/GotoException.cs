using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que se lanza cuando se encuentra una instrucción GOTO.
    /// Esta excepción se usa como mecanismo de control de flujo para implementar saltos.
    /// </summary>
    public class GotoException : Exception
    {
        /// <summary>
        /// Obtiene el nombre de la etiqueta a la que se debe saltar.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase GotoException.
        /// </summary>
        /// <param name="label">El nombre de la etiqueta a la que se debe saltar.</param>
        public GotoException(string label) : base("Goto instruction")
        {
            Label = label;
        }
    }
}