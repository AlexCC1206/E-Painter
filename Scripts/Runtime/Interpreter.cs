using System;
using System.Collections.Generic;
using System.Drawing;

namespace EPainter
{
    public class Interpreter : Expr.IVisitor<object>, Statement.IStatementVisitor<object>
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private Dictionary<string, int> labels = new Dictionary<string, int>();
        private List<Statement> statements;
        private int currentStatement = 0;
        private CanvasState canvasState;

        public Interpreter(int canvasSize, Dictionary<string, int> labels)
        {
            canvasState = new CanvasState(canvasSize);
            this.labels = labels;
        }

        public void Interpret(List<Statement> Statements)
        {
            statements = Statements;

            if (Statements == null || Statements.Count == 0)
            {
                throw new RuntimeError("The program contains no instructions.");
            }

            if (!(Statements[0] is Statement.Spawn))
            {
                throw new RuntimeError("The first command must be 'Spawn'.");
            }

            for (var i = 0; i < Statements.Count; i++)
            {
                if (Statements[i] is Statement.Label label)
                {
                    labels[label.Name.Lexeme] = 1;
                }
            }

            while (currentStatement < Statements.Count)
            {
                Execute(Statements[currentStatement]);
                currentStatement++;
            }
        }

        public void Execute(Statement statements)
        {
            statements.Accept(this);
        }

        public object VisitSpawnStatement(Statement.Spawn spawn)
        {
            var x = (int)Evaluate(spawn.X);
            var y = (int)Evaluate(spawn.Y);

            if (x < 0 || x >= canvasState.Size || y < 0 || y >= canvasState.Size)
            {
                throw new RuntimeError("Spawn position off the canvas");
            }

            canvasState.Position.Item1 = x;
            canvasState.Position.Item2 = y;
            return null;
        }

        public object VisitColorStatement(Statement.Color color)
        {
            var colorName = Evaluate(color.ColorName).ToString();
            canvasState.CurrentColor = colorName switch
            {

                "Red" => Color.Red,
                "Blue" => Color.Blue,
                "Yellow" => Color.Yellow,
                "Orange" => Color.Orange,
                "Purple" => Color.Purple,
                "Black" => Color.Black,
                "White" => Color.White,
                "Transparent" => Color.Transparent,

                _ => throw new RuntimeError($"Unknown color: {colorName}")
            };
            return null;
        }
        /*
                private bool IsValidColor(object color)
                {
                    string[] validColors = { "Red", "Blue", "Yellow", "Orange", "Purple", "Black", "White", "Transparent" };
                    return validColors.Contains(color);
                }*/

        public object VisitSizeStatement(Statement.Size size)
        {
            int size1 = (int)Evaluate(size.SizeValue);

            if (size1 <= 0)
            {
                throw new RuntimeError("Brush size must be positive");
            }

            if (size1 % 2 == 0)
            {
                canvasState.BrushSize = size1 - 1;
            }
            else
            {
                canvasState.BrushSize = size1;
            }

            return null;
        }

        public object VisitDrawLineStatement(Statement.DrawLine drawline)
        {
            var dirX = (int)Evaluate(drawline.DirX);
            var dirY = (int)Evaluate(drawline.DirY);
            var distance = (int)Evaluate(drawline.Distance);

            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
            {
                throw new RuntimeError($"Invalid directions: ({dirX}, {dirY})");
            }

            canvasState.DrawLine(dirX, dirY, distance);
            return true;
        }

        public object VisitDrawCircleStatement(Statement.DrawCircle drawCircle)
        {
            var dirX = (int)Evaluate(drawCircle.DirX);
            var dirY = (int)Evaluate(drawCircle.DirY);
            var radius = (int)Evaluate(drawCircle.Radius);

            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
            {
                throw new RuntimeError($"Invalid directions: ({dirX}, {dirY})");
            }

            canvasState.DrawCircle(dirX, dirY, radius);
            return null;
        }

        public object VisitDrawRectangleStatement(Statement.DrawRectangle drawRectangle)
        {
            var dirX = (int)Evaluate(drawRectangle.DirX);
            var dirY = (int)Evaluate(drawRectangle.DirY);
            var distance = (int)Evaluate(drawRectangle.Distance);
            var width = (int)Evaluate(drawRectangle.Width);
            var height = (int)Evaluate(drawRectangle.Height);

            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
            {
                throw new RuntimeError($"Invalid directions: ({dirX}, {dirY})");
            }

            canvasState.DrawRectangle(dirX, dirY, distance, width, height);
            return null;
        }

        public object VisitFillStatement(Statement.Fill fill)
        {

        }
        public object VisitGotoStatement(Statement.Goto @goto)
        {
            var condition = (bool)Evaluate(@goto.Condition);

            if (condition)
            {
                if (!labels.TryGetValue(@goto.label.Lexeme, out int targetLine))
                {
                    throw new RuntimeError($"Undefined label: {@goto.label.Lexeme}");
                }
                currentStatement = targetLine;
            }
            return null;
        }

        public object VisitAssignmentStatement(Statement.Assignment assignment)
        {
            object value = Evaluate(assignment.Value);
            variables[assignment.Name.Lexeme] = value;
            return null;
        }

        public object VisitBinaryExpr(Expr.Binary binary)
        {
            object left = Evaluate(binary.left);
            object right = Evaluate(binary.rigth);

            switch (binary.Operator.Type)
            {
                case TokenType.MIN:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left - (int)right;
                case TokenType.SUM:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left + (int)right;
                case TokenType.DIV:
                    if ((int)right == 0)
                    {
                        throw new RuntimeError(binary.Operator, "Division by zero.");
                    }
                    return (int)left / (int)right;
                case TokenType.MULT:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left * (int)right;
                case TokenType.MOD:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left % (int)right;
                case TokenType.POW:
                    CheckNumberOperand(binary.Operator, left, right);
                    return Math.Pow((int)left, (int)right);

                case TokenType.GREATER:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left > (int)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left >= (int)right;
                case TokenType.LESS:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left < (int)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(binary.Operator, left, right);
                    return (int)left <= (int)right;
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);

                case TokenType.AND:
                    return IsTruty(left) && IsTruty(right);
                case TokenType.OR:
                    return IsTruty(left) || IsTruty(right);

                default:
                    throw new RuntimeError($"Unknown operator: {binary.Operator.Lexeme}");
            }
        }

        public object VisitUnaryExpr(Expr.Unary unary)
        {
            object right = Evaluate(unary.right);

            switch (unary.Operator.Type)
            {
                case TokenType.MIN:
                    return -Convert.ToDouble(right);
                default:
                    throw new RuntimeError($"Unknown operator: {unary.Operator.Lexeme}");
            }
        }

        public object VisitLiteralExpr(Expr.Literal literal)
        {
            return literal.Value;
        }

        public object VisitVariableExpr(Expr.Variable variable)
        {
            if (!variables.TryGetValue(variable.Name.Lexeme, out object value))
            {
                throw new RuntimeError($"Undefined variable: {variable.Name.Lexeme}");
            }
            return value;
        }

        public object VisitFunctionCallExpr(Expr.FunctionCall functionCall)
        {
            /*
            var args = new List<object>();
            foreach (var arg in functionCall.Arguments)
            {
                args.Add(Evaluate(arg));
            }

            switch (functionCall.Name.Lexeme)
            {
                case "GetActualX":
                    return canvasState.Position.Item1;
                case "GetActualY":
                    return canvasState.Position.Item2;
                case "GetCanvasSize":
                    return canvasState.Size;
                case "GetColorCount":
                    if (args.Count != 5 || !(args[0] is string color) ||
                        !(args[1] is int) || !(args[2] is int) ||
                        !(args[3] is int) || !(args[4] is int))
                    {
                        throw new RuntimeError(functionCall.Name, "Invalid arguments for GetColorCount.");
                    }
                    int x1 = (int)args[1];
                    int y1 = (int)args[2];
                    int x2 = (int)args[3];
                    int y2 = (int)args[4];

                    if (x1 < 0 || x1 >= canvasState.Size || y1 < 0 || y1 >= canvasState.Size ||
                        x2 < 0 || x2 >= canvasState.Size || y2 < 0 || y2 >= canvasState.Size)
                    {
                        return 0;
                    }

                    int count = 0;
                    for (var x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    {
                        for (var y = Math.Min(y1, y2); y < Math.Max(y1, y2); y++)
                        {
                            if (canvasState.GetPixel(x, y) == Color.FromName(color))
                            {
                                count++;
                            }
                        }
                    }
                    return count;

                case "IsBrushColor":
                    if (args.Count != 1 || !(args[0] is string targetColor))
                    {
                        throw new RuntimeError(functionCall.Name, "Invalid argument for IsBrushColor.");
                    }
                    return canvasState.BrushColor == Color.FromName(targetColor) ? 1 : 0;

                case "IsBrushSize":
                    if (args.Count != 1 || !(args[0] is int size))
                    {
                        throw new RuntimeError(functionCall.Name, "Invalid argument for IsBrushSize.");
                    }
                    return canvasState.BrushSize == size ? 1 : 0;

                case "IsCanvasColor":
                    if (args.Count != 3 || !(args[0] is string checkColor) || !(args[1] is int) || !(args[2] is int))
                    {
                        throw new RuntimeError(functionCall.Name, "Invalid argument for IsCanvasColor");
                    }
                    int vertical = (int)args[1];
                    int horizontal = (int)args[2];

                    int x = canvasState.Position.Item1 + horizontal;
                    int y = canvasState.Position.Item2 + vertical;

                    if (x < 0 || x >= canvasState.Size || y < 0 || y >= canvasState.Size)
                    {
                        return 0;
                    }

                    return canvasState.GetPixel(x, y) == Color.FromName(checkColor) ? 1 : 0;

                default:
                    throw new RuntimeError(functionCall.Name, $"Function '{functionCall.Name.Lexeme}' not recognized.");
            }*/

            throw new RuntimeError(functionCall.Name, $"Undefined function '{functionCall.Name.Lexeme}'.");
        }

        public object VisitGroupingExpr(Expr.Grouping grouping)
        {
            return Evaluate(grouping.Expr);
        }

        private bool IsTruty(object obj)
        {
            if (obj is bool b) return b;
            return false;
        }

        private void CheckNumberOperand(Token Operator, object left, object right)
        {
            if (left is int && right is int) return;

            throw new RuntimeError(Operator, "Operands must be a numbers");
        }

        private void CheckNumberOperand(Token Operator, object right)
        {
            if (right is int) return;

            throw new RuntimeError(Operator, "Operand must be a number");
        }

        private bool isEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }
    }
}