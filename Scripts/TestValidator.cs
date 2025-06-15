using System;
using System.IO;
using EPainter.Core;

namespace EPainter
{
    public class TestValidator
    {        public static void RunTests()
        {
            string[] testFiles = new string[]
            {
                "SpawnValidationTest.txt",
                "NoSpawnTest.txt",
                "MultipleSpawnTest.txt",
                "CommandBeforeSpawnTest.txt",
                "LabelBeforeSpawnTest.txt",
                "NegativeBrushSizeTest.txt",   // Nueva prueba para tamaños negativos del pincel
                "InfiniteLoopTest.txt"         // Nueva prueba para ciclos infinitos
            };

            foreach (var file in testFiles)
            {
                Console.WriteLine($"\n===== Prueba: {file} =====");
                try
                {
                    string path = Path.Combine("Tests", file);
                    string code = File.ReadAllText(path);
                    
                    Console.WriteLine("Contenido del archivo:");
                    Console.WriteLine(code);
                    
                    // Escaneo léxico
                    Scanner scanner = new Scanner(code);
                    var tokens = scanner.scanTokens();
                    
                    if (ErrorReporter.HasErrors)
                    {
                        Console.WriteLine("Errores de análisis léxico:");
                        foreach (var error in ErrorReporter.Errors)
                            Console.WriteLine(error);
                        
                        ErrorReporter.Reset();
                        continue;
                    }
                    
                    // Análisis sintáctico
                    Parser parser = new Parser(tokens);
                    var statements = parser.Parse();
                    
                    if (ErrorReporter.HasErrors)
                    {
                        Console.WriteLine("Errores de análisis sintáctico:");
                        foreach (var error in ErrorReporter.Errors)
                            Console.WriteLine(error);
                        
                        ErrorReporter.Reset();
                        continue;
                    }
                    
                    // Interpretación
                    var interpreter = new Interpreter();
                    var canvas = new Canvas(20); // Tamaño de prueba
                    
                    try
                    {
                        interpreter.Interpret(canvas, statements);
                        Console.WriteLine("Ejecución exitosa ✓");
                    }
                    catch (RuntimeError ex)
                    {
                        Console.WriteLine($"Error en tiempo de ejecución: {ex.Message} ✗");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error inesperado: {ex.Message} ✗");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar el archivo: {ex.Message} ✗");
                }
                
                ErrorReporter.Reset();
            }
            
            Console.WriteLine("\nPruebas completadas.");
        }
    }
}
