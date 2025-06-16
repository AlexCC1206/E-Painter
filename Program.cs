using Godot;
using System;

namespace EPainter
{
    public static class Program
    {
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
