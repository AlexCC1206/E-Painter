using System;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace EPainter
{
    public class EPainter
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length > 1)
                {
                    Console.WriteLine("Usage: jlox [script]");
                    Environment.Exit(64);
                }
                else if (args.Length == 1)
                {
                    RunFile(args[0]);
                }
                else
                {
                    RunPrompt();
                }
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(65);
            }
        }

        private static void RunFile(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                string source = Encoding.UTF8.GetString(bytes);
                Run(source);

                // Indicate an error in the exit code.
                if (hadError)
                {
                    Environment.Exit(65);
                }
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"[ERROR] Unable to read file: {path}. {ex.Message}");
                hadError = true;
            }
        }

        private static void RunPrompt()
        {
            using (StreamReader input = new StreamReader(Console.OpenStandardInput()))
            {
                while (true)
                {
                    Console.Write("> ");
                    string line = input.ReadLine();
                    if (line == null) break;

                    Run(line);
                    hadError = false; // Reiniciar hadError después de procesar la línea
                }
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            Expr expr = parser.parse();

            if(hadError) return;
            // For now, just print the tokens.
            Console.WriteLine(new AstPrinter().Print(expr));
        }

        private static bool hadError = false;

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }

        public static void Error(Token token, string message)
        {
            Console.WriteLine($"[Línea {token.line}] Error en '{token.lexeme}': {message}");
        }
    }
}
