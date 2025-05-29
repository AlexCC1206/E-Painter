
using System;
using System.Collections.Generic;

namespace EPainter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var commands = new List<Command>
    {
        new Command.Spawn(
            new Expr.Literal(0),
            new Expr.Literal(0)),

        new Command.Color(
            new Expr.Literal("Blue")),

        new Command.Assignment(
            new Token(TokenType.IDENTIFIER, "distance", null, 3),
            new Expr.Literal(10)),

        new Command.DrawLine(
            new Expr.Literal(1),
            new Expr.Literal(0),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "distance", null, 5)))
    };

            var printer = new AstPrinter();
            Console.WriteLine(printer.PrintProgram(commands));
        }
    }
}