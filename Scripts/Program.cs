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
            {                Console.WriteLine("Starting E-Painter...");
                Console.WriteLine("This application must be run through the Godot engine.");
                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error starting the application: {ex.Message}");
            }
        }
    }
}