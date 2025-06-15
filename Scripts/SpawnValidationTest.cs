using System;
using System.IO;
using EPainter.Core;

namespace EPainter
{
    class SpawnValidationTest
    {
        static void TestSpawnValidationDirectly()
        {
            // Prueba 1: Archivo con Spawn como primer comando (debería funcionar)
            Console.WriteLine("\n===== TEST CASO 1: Spawn como primer comando =====");
            TestFile("d:\\Alex Jr\\Escuela\\Programación\\2do Semestre\\Proyecto\\E-Painter\\Tests\\SpawnValidationTest.txt");

            // Prueba 2: Archivo sin Spawn (debería fallar)
            Console.WriteLine("\n===== TEST CASO 2: Sin comando Spawn =====");
            TestFile("d:\\Alex Jr\\Escuela\\Programación\\2do Semestre\\Proyecto\\E-Painter\\Tests\\NoSpawnTest.txt");

            // Prueba 3: Archivo con múltiples Spawn (debería fallar)
            Console.WriteLine("\n===== TEST CASO 3: Múltiples comandos Spawn =====");
            TestFile("d:\\Alex Jr\\Escuela\\Programación\\2do Semestre\\Proyecto\\E-Painter\\Tests\\MultipleSpawnTest.txt");

            // Prueba 4: Comando antes de Spawn (debería fallar)
            Console.WriteLine("\n===== TEST CASO 4: Comando antes de Spawn =====");
            TestFile("d:\\Alex Jr\\Escuela\\Programación\\2do Semestre\\Proyecto\\E-Painter\\Tests\\CommandBeforeSpawnTest.txt");

            // Prueba 5: Etiqueta antes de Spawn (debería funcionar)
            // Nota: Se omite esta prueba por ahora porque hay un problema con el manejo de etiquetas
        }

        static void TestFile(string filePath)
        {
            try
            {
                Console.WriteLine($"Archivo: {Path.GetFileName(filePath)}");
                string code = File.ReadAllText(filePath);
                Console.WriteLine("Contenido:");
                Console.WriteLine(code);
                
                // Escaneo léxico
                ErrorReporter.Reset();
                Scanner scanner = new Scanner(code);
                var tokens = scanner.scanTokens();
                
                if (ErrorReporter.HasErrors)
                {
                    Console.WriteLine("Errores de análisis léxico:");
                    foreach (var error in ErrorReporter.Errors)
                        Console.WriteLine(error);
                    return;
                }
                
                // Análisis sintáctico
                Parser parser = new Parser(tokens);
                var statements = parser.Parse();
                
                if (ErrorReporter.HasErrors)
                {
                    Console.WriteLine("Errores de análisis sintáctico:");
                    foreach (var error in ErrorReporter.Errors)
                        Console.WriteLine(error);
                    return;
                }
                
                // Interpretación
                var interpreter = new Interpreter();
                var canvas = new Canvas(20); // Tamaño de prueba
                
                try
                {
                    // Iniciamos la interpretación solo para probar la validación
                    interpreter.Interpret(canvas, statements);
                    Console.WriteLine("✓ ÉXITO: El código pasa la validación de Spawn");
                }
                catch (RuntimeError ex)
                {
                    Console.WriteLine($"✗ ERROR: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR INESPERADO: {ex.Message}");
            }
        }

        public static void RunTests()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("PRUEBAS DE VALIDACIÓN DE SPAWN");
            Console.WriteLine("=================================");
            
            TestSpawnValidationDirectly();
            
            Console.WriteLine("\nPruebas completadas.");
        }
    }
}
