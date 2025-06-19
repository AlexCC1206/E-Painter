using System;

namespace EPainter.Core
{
    /// <summary>
    /// Representa una excepción que se lanza cuando se encuentra una instrucción GOTO.
    /// Esta excepción se usa como mecanismo de control de flujo para implementar saltos.
    /// </summary>
    public class GotoException : EPainterException
    {
        /// <summary>
        /// Obtiene el nombre de la etiqueta a la que se debe saltar.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase GotoException.
        /// </summary>
        /// <param name="label">El nombre de la etiqueta a la que se debe saltar.</param>
        /// <param name="line">El número de línea donde ocurrió la instrucción GOTO (opcional).</param>
        public GotoException(string label, int? line = null) : base("Goto instruction", line)
        {
            Label = label;
        }
        
        /// <summary>
        /// Obtiene el tipo de error específico para mostrar en mensajes.
        /// </summary>
        protected override string GetErrorType() => "Flow Control";
    }
}