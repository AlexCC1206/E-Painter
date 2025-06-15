using System;
using System.IO;
using EPainter.Core;

namespace EPainter
{
    class Program
    {        static void Main(string[] args)
        {
            // Revisar si se solicita la prueba del validador de Spawn
            if (args.Length > 0 && args[0] == "/test-spawn-validation")
            {
                Console.WriteLine("Ejecutando pruebas de validación de Spawn...");
                SpawnValidationTest.RunTests();
                return;
            }
              // Revisar si se solicita la prueba del validador general
            if (args.Length > 0 && args[0] == "/test-validator")
            {
                Console.WriteLine("Ejecutando pruebas de validación generales...");
                TestValidator.RunTests();
                return;
            }
            
            // Revisar si se solicita la prueba de las nuevas validaciones semánticas
            if (args.Length > 0 && args[0] == "/test-semantic")
            {
                Console.WriteLine("Ejecutando pruebas de validación semántica...");
                NewValidationsTest.RunTests();
                return;
            }
            
            // Ejecutar el ejemplo básico
            PruebaBasica();
            
            // Ejecutar pruebas desde un archivo
            if (args.Length > 0)
            {
                string filePath = args[0];
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"\n====== PRUEBA DESDE ARCHIVO: {filePath} ======");
                    string codigo = File.ReadAllText(filePath);
                    RunTest(codigo);
                }
            }
            else
            {
                // Ejecutar pruebas adicionales
                PruebasAvanzadas();
            }
        }

        static void PruebaBasica()
        {            Console.WriteLine("====== PRUEBA BÁSICA ======");
            // Código del ejemplo simplificado y corregido
            string sourceCode = @"Spawn(0, 0)
Color(""Black"")
n <- 5
k <- 3 + 3 * 10
n <- k * 2
DrawLine(1, 0, 10)
Color(""Blue"")
DrawCircle(0, 0, 5)
";

            RunTest(sourceCode);
        }

        static void PruebasAvanzadas()
        {
            Console.WriteLine("\n====== PRUEBAS AVANZADAS ======");
            
            // Prueba 1: Dibujar un cuadrado
            string sourceCode1 = @"Spawn(5, 5)
Color(""Black"")
Size(1)
DrawLine(1, 0, 5)
DrawLine(0, 1, 5)
DrawLine(-1, 0, 5)
DrawLine(0, -1, 5)";
            
            Console.WriteLine("\n--- Prueba 1: Dibujar un cuadrado ---");
            RunTest(sourceCode1);
            
            // Prueba 2: Uso de etiquetas y GoTo
            string sourceCode2 = @"Spawn(10, 10)
Color(""Black"")
i <- 0
loop1
DrawCircle(0, 0, i)
i <- i + 1
GoTo[loop_end](i > 3)
GoTo[loop1](i <= 3)
loop_end
Color(""Blue"")";
            
            Console.WriteLine("\n--- Prueba 2: Uso de etiquetas y GoTo ---");
            RunTest(sourceCode2);
            
            // Prueba 3: Usar rectángulos y fill
            string sourceCode3 = @"Spawn(5, 5)
Color(""Black"")
DrawRectangle(0, 0, 0, 5, 5)
Spawn(7, 7)
Color(""Blue"")
Fill()";
            
            Console.WriteLine("\n--- Prueba 3: Usar rectángulos y fill ---");
            RunTest(sourceCode3);
        }        static void RunTest(string sourceCode)
        {
            // Crear canvas inicial
            var canvas = new Canvas(20); // Tamaño 20x20
            Console.WriteLine("Canvas inicial:");
            canvas.Print();

            // Reiniciar el sistema de reporte de errores
            ErrorReporter.Reset();

            // Scanner
            var scanner = new Scanner(sourceCode);
            var tokens = scanner.scanTokens();

            // Si hay errores de escaneo, mostrarlos y salir
            if (ErrorReporter.HasErrors)
            {
                Console.WriteLine("\nErrores sintácticos detectados:");
                ErrorReporter.PrintAllErrors();
                return;
            }

            // Parser
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            // Si hay errores de parseo, mostrarlos y salir
            if (ErrorReporter.HasErrors)
            {
                Console.WriteLine("\nErrores sintácticos/semánticos detectados:");
                ErrorReporter.PrintAllErrors();
                return;
            }

            // Intérprete
            var interpreter = new Interpreter();
            try
            {
                interpreter.Interpret(canvas, statements);
                
                // Si hay errores de tiempo de ejecución, mostrarlos
                if (ErrorReporter.HasRuntimeErrors)
                {
                    Console.WriteLine("\nErrores en tiempo de ejecución:");
                    ErrorReporter.PrintAllErrors();
                    Console.WriteLine("\nResultado parcial (hasta el error):");
                }
                else
                {
                    Console.WriteLine("\nEjecución completada sin errores.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError durante la ejecución: {ex.Message}");
            }

            // Mostrar resultado
            Console.WriteLine("\nCanvas final:");
            canvas.Print();
        }
    }
}