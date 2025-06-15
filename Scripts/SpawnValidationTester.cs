using System;
using System.IO;
using System.Collections.Generic;
using EPainter.Core;

namespace EPainter
{
    public class SpawnValidationTester
    {
        public static void RunTests()
        {
            string[] testFiles = new string[]
            {
                "SpawnValidationTest.txt",
                "NoSpawnTest.txt",
                "MultipleSpawnTest.txt",
                "CommandBeforeSpawnTest.txt",
                "LabelBeforeSpawnTest.txt"
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
                        // Llamamos solo a ValidateSpawnIsFirstAndUnique a través de reflexión
                        var method = typeof(Interpreter).GetMethod("ValidateSpawnIsFirstAndUnique", 
                            System.Reflection.BindingFlags.NonPublic | 
                            System.Reflection.BindingFlags.Instance);
                        
                        // Preparamos el intérprete
                        typeof(Interpreter).GetField("canvas", 
                            System.Reflection.BindingFlags.NonPublic | 
                            System.Reflection.BindingFlags.Instance)
                            .SetValue(interpreter, canvas);
                        
                        typeof(Interpreter).GetField("statements", 
                            System.Reflection.BindingFlags.NonPublic | 
                            System.Reflection.BindingFlags.Instance)
                            .SetValue(interpreter, statements);
                        
                        // Invocamos el método de validación
                        method.Invoke(interpreter, null);
                        
                        Console.WriteLine("Validación exitosa ✓");
                    }
                    catch (Exception ex)
                    {
                        var innerEx = ex.InnerException;
                        if (innerEx is RuntimeError)
                        {
                            Console.WriteLine($"Error en tiempo de ejecución: {innerEx.Message} ✗");
                        }
                        else
                        {
                            Console.WriteLine($"Error inesperado: {ex.Message} ✗");
                        }
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
