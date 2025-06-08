using System;
using System.Collections.Generic;
using EPainter;

public class Program
{
    public static void Main(string[] args)
    {
        string sourceCode = @"
        Spawn(0, 0)
        Color(Red)
        Size(3)
        x <- 10
        GoTo [label] (x > 5)
        ";

        var scanner = new Scanner(sourceCode);
        List<Token> tokens = scanner.scanTokens();

        var parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();


        foreach (var stmt in statements)
        {
            Console.WriteLine(stmt);
        }
    }
}