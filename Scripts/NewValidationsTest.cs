using System;
using EPainter.Core;
using System.IO;

namespace EPainter
{
    public class NewValidationsTest
    {        public static void RunTests()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("PRUEBAS DE VALIDACIÓN SEMÁNTICA");
            Console.WriteLine("======================================");
            
            // Prueba 1: Tamaño del pincel negativo
            TestBrushSizeValidation();
            
            // Prueba 2: Ciclo infinito
            TestInfiniteLoopDetection();
            
            // Prueba 3: Tipo de datos incorrecto en Color
            TestColorTypeValidation();
            
            // Prueba 4: Consistencia de tipos en operaciones
            TestTypeConsistency();
            
            Console.WriteLine("\nPruebas de validación semántica completadas.");
        }
        
        private static void TestBrushSizeValidation()
        {
            Console.WriteLine("\n===== TEST: Validación de tamaño del pincel =====");
            RunTest("NegativeBrushSizeTest.txt", "Se espera que rechace tamaños de pincel negativos");
        }
        
        private static void TestInfiniteLoopDetection()
        {
            Console.WriteLine("\n===== TEST: Detección de ciclos infinitos =====");
            RunTest("InfiniteLoopTest.txt", "Se espera que detecte un ciclo infinito");
        }
          private static void TestColorTypeValidation()
        {
            Console.WriteLine("\n===== TEST: Validación de tipo en Color =====");            string testCode = @"
Spawn(10, 10)
Color(5)  // Debería generar error al usar un número en lugar de un string
DrawLine(1, 0, 5)
";
            
            File.WriteAllText(Path.Combine("Tests", "ColorTypeTest.txt"), testCode);
            RunTest("ColorTypeTest.txt", "Se espera que rechace un número como argumento de Color");
        }
        
        private static void TestTypeConsistency()
        {
            Console.WriteLine("\n===== TEST: Consistencia de tipos en operaciones =====");
            RunTest("TypeConsistencyTest.txt", "Se espera que detecte tipos inconsistentes en operaciones");
        }
        
        private static void RunTest(string fileName, string description)
        {
            Console.WriteLine($"Prueba: {description}");
            try
            {
                string path = Path.Combine("Tests", fileName);
                if (!File.Exists(path))
                {
                    Console.WriteLine($"El archivo {fileName} no existe.");
                    return;
                }
                
                string code = File.ReadAllText(path);
                Console.WriteLine("\nContenido del archivo:");
                Console.WriteLine(code);
                
                ErrorReporter.Reset();
                Scanner scanner = new Scanner(code);
                var tokens = scanner.scanTokens();
                
                if (ErrorReporter.HasErrors)
                {
                    Console.WriteLine("\nErrores de análisis léxico:");
                    foreach (var error in ErrorReporter.Errors)
                        Console.WriteLine(error);
                    return;
                }
                
                Parser parser = new Parser(tokens);
                var statements = parser.Parse();
                
                if (ErrorReporter.HasErrors)
                {
                    Console.WriteLine("\nErrores de análisis sintáctico:");
                    foreach (var error in ErrorReporter.Errors)
                        Console.WriteLine(error);
                    return;
                }
                
                var interpreter = new Interpreter();
                var canvas = new Canvas(20); // Tamaño de prueba
                
                try
                {
                    interpreter.Interpret(canvas, statements);
                    Console.WriteLine("\n✅ La ejecución fue exitosa (no se esperaba un error).");
                }
                catch (RuntimeError ex)
                {
                    Console.WriteLine($"\n❌ Error detectado (esperado): {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❓ Error inesperado: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❓ Error al ejecutar la prueba: {ex.Message}");
            }
        }
    }
}
