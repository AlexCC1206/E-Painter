using System;
using static EPainter.Core.Expr;

namespace EPainter.Core
{
    public class ExprVisitor : IExprVisitor<object>
    {
        private readonly Interpreter interpreter;

        public ExprVisitor(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public object VisitLiteral(Literal expr)
        {
            return expr.Value;
        }

        public object VisitVariable(Variable expr)
        {
            return interpreter.GetVariable(expr.Name);
        }

        public object VisitBinary(Binary expr)
        {
            object left = Visit(expr.Left);
            object right = Visit(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.SUM:
                    return Convert.ToInt32(left) + Convert.ToInt32(right);
                case TokenType.MIN:
                    return Convert.ToInt32(left) - Convert.ToInt32(right);
                case TokenType.MULT:
                    return Convert.ToInt32(left) * Convert.ToInt32(right);
                case TokenType.DIV:
                    if (Convert.ToInt32(right) == 0) throw new RuntimeError("División por cero.");
                    return Convert.ToInt32(left) / Convert.ToInt32(right);
                case TokenType.POW:
                    return Math.Pow(Convert.ToInt32(left), Convert.ToInt32(right));
                case TokenType.MOD:
                    return Convert.ToInt32(left) % Convert.ToInt32(right);
                case TokenType.EQUAL_EQUAL:
                    return Equals(left, right);
                case TokenType.BANG_EQUAL:
                    return !Equals(left, right);
                case TokenType.LESS:
                    return Convert.ToInt32(left) < Convert.ToInt32(right);
                case TokenType.LESS_EQUAL:
                    return Convert.ToInt32(left) <= Convert.ToInt32(right);
                case TokenType.GREATER:
                    return Convert.ToInt32(left) > Convert.ToInt32(right);
                case TokenType.GREATER_EQUAL:
                    return Convert.ToInt32(left) >= Convert.ToInt32(right);

                case TokenType.AND:
                    return (bool)left && (bool)right;
                case TokenType.OR:
                    return (bool)left || (bool)right;

                default:
                    throw new RuntimeError("Unknown operator: " + expr.Op.Lexeme);
            }
        }
          public object VisitUnary(Unary expr)
        {
            object right = Visit(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.MIN:
                    return -Convert.ToInt32(right);
                default:
                    throw new RuntimeError("Unknown unary operator: " + expr.Op.Lexeme);
            }
        }

        public object VisitGrouping(Grouping expr)
        {
            return Visit(expr.Expression);
        }

        public object VisitCall(Call expr)
        {
            switch (expr.FunctionName)
            {
                case "GetActualX":
                    return interpreter.GetActualX();
                case "GetActualY":
                    return interpreter.GetActualY();
                case "GetCanvasSize":
                    return interpreter.GetCanvasSize();
                case "IsBrushColor":
                    return interpreter.IsBrushColor((string)Visit(expr.Arguments[0]));
                case "IsBrushSize":
                    return interpreter.IsBrushSize((int)Visit(expr.Arguments[0]));
                case "IsCanvasColor":
                    return interpreter.IsCanvasColor(
                        (string)Visit(expr.Arguments[0]),
                        (int)Visit(expr.Arguments[1]),
                        (int)Visit(expr.Arguments[2])
                    );
                case "GetColorCount":
                    return interpreter.GetColorCount(
                        (string)Visit(expr.Arguments[0]),
                        (int)Visit(expr.Arguments[1]),
                        (int)Visit(expr.Arguments[2]),
                        (int)Visit(expr.Arguments[3]),
                        (int)Visit(expr.Arguments[4])
                    );
                default:
                    throw new RuntimeError($"Function '{expr.FunctionName}' not found.");
            }
        }

        private object Visit(Expr expr)
        {
            return expr.Accept(this);
        }
    }
}