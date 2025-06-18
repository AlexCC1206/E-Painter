using Godot;
using System;

namespace EPainter
{
    /// <summary>
    /// Clase principal para la entrada de la aplicación E-Painter.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación E-Painter.
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Iniciando E-Painter...");
                Console.WriteLine("Esta aplicación debe ejecutarse a través del motor Godot.");
                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al iniciar la aplicación: {ex.Message}");
            }
        }
    }
}
