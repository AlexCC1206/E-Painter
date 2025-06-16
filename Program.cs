using Godot;
using System;

namespace EPainter
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Este método sirve como punto de entrada para la aplicación .NET
            // Cuando se ejecuta como aplicación de Godot, este método no se usa
            // ya que Godot maneja la inicialización del motor
            try
            {
                // Para ejecutables independientes, simplemente mostramos un mensaje
                // y salimos con éxito
                Console.WriteLine("Iniciando E-Painter...");
                Console.WriteLine("Esta aplicación debe ejecutarse a través del motor Godot.");
                
                // En un entorno real, podrías agregar aquí código para iniciar Godot
                // programáticamente si fuera necesario
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al iniciar la aplicación: {ex.Message}");
            }
        }
    }
}
