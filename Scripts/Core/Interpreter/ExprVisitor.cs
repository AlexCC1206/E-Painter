using System;

namespace EPainter.Core
{
    /// <summary>
    /// Implementa el patrón Visitor para evaluar expresiones del lenguaje E-Painter.
    /// </summary>
    public class ExprVisitor : IExprVisitor<object>
    {
        #region Campos
        /// <summary>
        /// La instancia del intérprete que utiliza este visitante.
        /// </summary>
        private readonly Interpreter Interpreter;
        #endregion

        #region Inicialización
        /// <summary>
        /// Inicializa una nueva instancia de la clase ExprVisitor.
        /// </summary>
        /// <param name="interpreter">La instancia del intérprete a utilizar.</param>
        public ExprVisitor(Interpreter interpreter)
        {
            Interpreter = interpreter;
        }
        #endregion

        #region Evaluación de Expresiones
        /// <summary>
        /// Evalúa una expresión literal.
        /// </summary>
        /// <param name="expr">La expresión literal a evaluar.</param>
        /// <returns>El valor literal almacenado.</returns>
        public object VisitLiteral(Literal expr)
        {
            return expr.Value;
        }

        /// <summary>
        /// Evalúa una expresión de variable.
        /// </summary>
        /// <param name="expr">La expresión de variable a evaluar.</param>
        /// <returns>El valor almacenado en la variable.</returns>
        public object VisitVariable(Variable expr)
        {
            if (!Interpreter.Variables.TryGetValue(expr.Name, out var value))
            {
                Interpreter.Error(1 /*expr.Line*/, $"Undefined variable '{expr.Name}'.");
                return null;
            }

            return value;
        }

        /// <summary>
        /// Evalúa una expresión binaria.
        /// </summary>
        /// <param name="expr">La expresión binaria a evaluar.</param>
        /// <returns>El resultado de la operación binaria.</returns>
        /// <exception cref="RuntimeError">Se lanza si hay un error en la operación, como división por cero.</exception>
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
                    if (Convert.ToInt32(right) == 0)
                    {
                        
                        /*
                        RuntimeError divError = new RuntimeError(expr.Op, "Division by zero.");
                        ErrorReporter.RuntimeError(divError);
                        throw divError;*/
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
                    RuntimeError binOpError = new RuntimeError(expr.Op, "Unknown operator: " + expr.Op.Lexeme);
                    ErrorReporter.RuntimeError(binOpError);
                    throw binOpError;
            }
        }

        /// <summary>
        /// Evalúa una expresión unaria.
        /// </summary>
        /// <param name="expr">La expresión unaria a evaluar.</param>
        /// <returns>El resultado de la operación unaria.</returns>
        /// <exception cref="RuntimeError">Se lanza si hay un operador unario desconocido.</exception>
        public object VisitUnary(Unary expr)
        {
            object right = Visit(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.MIN:
                    CheckNumberOperand(expr.Op, right);
                    return -Convert.ToInt32(right);
                default:
                    RuntimeError unaryOpError = new RuntimeError(expr.Op, "Unknown unary operator: " + expr.Op.Lexeme);
                    ErrorReporter.RuntimeError(unaryOpError);
                    throw unaryOpError;
            }
        }

        /// <summary>
        /// Evalúa una expresión agrupada.
        /// </summary>
        /// <param name="expr">La expresión agrupada a evaluar.</param>
        /// <returns>El resultado de evaluar la expresión dentro del grupo.</returns>
        public object VisitGrouping(Grouping expr)
        {
            return Visit(expr.Expression);
        }

        /// <summary>
        /// Evalúa una llamada a función.
        /// </summary>
        /// <param name="expr">La expresión de llamada a función a evaluar.</param>
        /// <returns>El resultado de la llamada a función.</returns>
        /// <exception cref="RuntimeError">Se lanza si la función no se encuentra.</exception>
        public object VisitCall(Call expr)
        {
            switch (expr.FunctionName)
            {
                case "GetActualX":
                    if (expr.Arguments.Count != 0)
                    {
                        RuntimeError error = new RuntimeError("'GetActualX()' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.GetActualX();

                case "GetActualY":
                    if (expr.Arguments.Count != 0)
                    {
                        RuntimeError error = new RuntimeError("'GetActualY()' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.GetActualY();

                case "GetCanvasSize":
                    if (expr.Arguments.Count != 0)
                    {
                        RuntimeError error = new RuntimeError("'GetCanvasSize()' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.GetCanvasSize();

                case "IsBrushColor":
                    if (expr.Arguments.Count != 1 || !(Visit(expr.Arguments[0]) is string))
                    {
                        RuntimeError error = new RuntimeError("'IsBrushColor(string color)' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.IsBrushColor((string)Visit(expr.Arguments[0]));

                case "IsBrushSize":
                    if (expr.Arguments.Count != 1 || !(Visit(expr.Arguments[0]) is int))
                    {
                        RuntimeError error = new RuntimeError("'IsBrushSize(int size)' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.IsBrushSize((int)Visit(expr.Arguments[0]));

                case "IsCanvasColor":
                    if (expr.Arguments.Count != 3 || !(Visit(expr.Arguments[0]) is string) || !(Visit(expr.Arguments[1]) is int) || !(Visit(expr.Arguments[2]) is int))
                    {
                        RuntimeError error = new RuntimeError("'IsCanvasColor(string color, int vertical, int horizontal)' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.IsCanvasColor(
                        (string)Visit(expr.Arguments[0]),
                        (int)Visit(expr.Arguments[1]),
                        (int)Visit(expr.Arguments[2])
                    );

                case "GetColorCount":
                    if (expr.Arguments.Count != 5 || !(Visit(expr.Arguments[0]) is string) || !(Visit(expr.Arguments[1]) is int) || !(Visit(expr.Arguments[2]) is int) || !(Visit(expr.Arguments[3]) is int) || !(Visit(expr.Arguments[4]) is int))
                    {
                        RuntimeError error = new RuntimeError("'GetColorCount (string color, int x1, int y1, int x2, int y2)' no acepta argumentos.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    return Interpreter.GetColorCount(
                        (string)Visit(expr.Arguments[0]),
                        (int)Visit(expr.Arguments[1]),
                        (int)Visit(expr.Arguments[2]),
                        (int)Visit(expr.Arguments[3]),
                        (int)Visit(expr.Arguments[4])
                    );
                default:
                    RuntimeError funcError = new RuntimeError($"Function '{expr.FunctionName}' not found.");
                    ErrorReporter.RuntimeError(funcError);
                    throw funcError;
            }
        }
        #endregion

        #region Utilidades
        /// <summary>
        /// Verifica que el operando sea un número.
        /// </summary>
        /// <param name="op">El token del operador.</param>
        /// <param name="operand">El operando a verificar.</param>
        /// <exception cref="RuntimeError">Se lanza si el operando no es un número.</exception>
        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is int) return;
            RuntimeError typeError = new RuntimeError(op, "The operand must be a number");
            ErrorReporter.RuntimeError(typeError);
            throw typeError;
        }

        /// <summary>
        /// Verifica que ambos operandos de una operación sean números.
        /// </summary>
        /// <param name="op">El token del operador.</param>
        /// <param name="left">El operando izquierdo.</param>
        /// <param name="right">El operando derecho.</param>
        /// <exception cref="RuntimeError">Se lanza si alguno de los operandos no es un número.</exception>
        private void CheckNumberOperand(Token op, object left, object right)
        {
            if (left is int && right is int) return;
            RuntimeError typeError = new RuntimeError(op, "Operands must be numbers");
            ErrorReporter.RuntimeError(typeError);
            throw typeError;
        }

        /// <summary>
        /// Determina si un objeto es "verdadero" en el contexto de E-Painter.
        /// </summary>
        /// <param name="obj">El objeto a evaluar.</param>
        /// <returns>True si el objeto es "verdadero", de lo contrario, false.</returns>
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool boolObj) return boolObj;
            if (obj is int intObj) return intObj != 0;
            return true;
        }

        /// <summary>
        /// Visita una expresión y llama al método de aceptación correspondiente.
        /// </summary>
        /// <param name="expr">La expresión a visitar.</param>
        /// <returns>El resultado de la visita.</returns>
        private object Visit(Expr expr)
        {
            return expr.Accept(this);
        }
        #endregion
    }
}