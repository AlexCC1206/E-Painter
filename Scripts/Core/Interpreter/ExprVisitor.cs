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
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) + Convert.ToInt32(right);
                case TokenType.MIN:
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) - Convert.ToInt32(right);
                case TokenType.MULT:
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) * Convert.ToInt32(right);
                case TokenType.DIV:
                    CheckNumberOperand(expr.Op, left, right);
                    if (Convert.ToInt32(right) == 0) {
                        RuntimeError divError = new RuntimeError(expr.Op, "División por cero.");
                        ErrorReporter.RuntimeError(divError);
                        throw divError;
                    }
                    return Convert.ToInt32(left) / Convert.ToInt32(right);
                case TokenType.POW:
                    CheckNumberOperand(expr.Op, left, right);
                    return Math.Pow(Convert.ToInt32(left), Convert.ToInt32(right));
                case TokenType.MOD:
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) % Convert.ToInt32(right);
                case TokenType.EQUAL_EQUAL:
                    return Equals(left, right);
                case TokenType.BANG_EQUAL:
                    return !Equals(left, right);
                case TokenType.LESS:
                    return Convert.ToInt32(left) < Convert.ToInt32(right);
                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) <= Convert.ToInt32(right);
                case TokenType.GREATER:
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) > Convert.ToInt32(right);
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(expr.Op, left, right);
                    return Convert.ToInt32(left) >= Convert.ToInt32(right);

                case TokenType.AND:
                    return IsTruthy(left) && IsTruthy(right);
                case TokenType.OR:
                    return IsTruthy(left) || IsTruthy(right);

                default:
                    RuntimeError binOpError = new RuntimeError(expr.Op, "Operador desconocido: " + expr.Op.Lexeme);
                    ErrorReporter.RuntimeError(binOpError);
                    throw binOpError;
            }
        }
        public object VisitUnary(Unary expr)
        {
            object right = Visit(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.MIN:
                    CheckNumberOperand(expr.Op, right);
                    return -Convert.ToInt32(right);
                default:
                    RuntimeError unaryOpError = new RuntimeError(expr.Op, "Operador unario desconocido: " + expr.Op.Lexeme);
                    ErrorReporter.RuntimeError(unaryOpError);
                    throw unaryOpError;
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
                    RuntimeError funcError = new RuntimeError($"Función '{expr.FunctionName}' no encontrada.");
                    ErrorReporter.RuntimeError(funcError);
                    throw funcError;
            }
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is int) return;
            RuntimeError typeError = new RuntimeError(op, "El operando debe ser un número");
            ErrorReporter.RuntimeError(typeError);
            throw typeError;
        }

        private void CheckNumberOperand(Token op, object left, object right)
        {
            if (left is int && right is int) return;
            RuntimeError typeError = new RuntimeError(op, "Los operandos deben ser números");
            ErrorReporter.RuntimeError(typeError);
            throw typeError;
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool boolObj) return boolObj;
            if (obj is int intObj) return intObj != 0;
            return true;
        }

        private object Visit(Expr expr)
        {
            return expr.Accept(this);
        }
    }
}