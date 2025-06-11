using System;
using System.Collections.Generic;
using System.Drawing;
using EPainter.UI;

namespace EPainter.Core
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        public Environment Globals { get; } = new Environment();
        private Environment environment;
        private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        public Point Position { get; private set; }
        public Color CurrentColor { get; private set; } = Color.Transparent;
        public int BrushSize { get; private set; } = 1;

        public Canvas canvas;

        private Dictionary<string, int> labels = new Dictionary<string, int>();

        private List<Stmt> statements;
        private int currentStatement = 0;

        public Interpreter(int canvasSize)
        {
            environment = Globals;
            canvas = new Canvas(canvasSize);

            InitalizeNativeFunctions();
        }

        private void InitalizeNativeFunctions()
        {
            Globals.Define("GetActualX", new GetActualX());
            Globals.Define("GetActualY", new GetActualY());
            Globals.Define("GetCanvasSize", new GetCanvasSize());
            Globals.Define("GetColorCount", new GetColorCount());
            Globals.Define("IsBrushColor", new IsBrushColor());
            Globals.Define("IsBrushSize", new IsBrushSize());
            Globals.Define("IsCanvasColor", new IsCanvasColor());
        }

        /*
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                // Primero hacer análisis semántico
                var resolver = new Resolver(this);
                resolver.Resolve(statements);

                if (ErrorReporter.HasErrors) return;

                // Ejecutar si no hay errores
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                ErrorReporter.RuntimeError(error);
            }
        }*/

        public void Interpret(List<Stmt> statements)
        {
            this.statements = statements;
            currentStatement = 0;

            RegistrerLabels();

            try
            {
                while (currentStatement < statements.Count)
                {
                    Execute(statements[currentStatement]);
                    currentStatement++;
                }
            }
            catch (RuntimeError error)
            {
                ErrorReporter.RuntimeError(error);
            }
        }

        private void RegistrerLabels()
        {
            for (var i = 0; i < statements.Count; i++)
            {
                if (statements[i] is Stmt.Label label)
                {
                    labels[label.Name.Lexeme] = i;
                }
            }
        }

        public void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private object Evaluate(Expr expr)
        {
            try
            {
                return expr.Accept(this);
            }
            catch (RuntimeError error)
            {
                ErrorReporter.RuntimeError(error);
                throw;
            }

        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.SUM:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left + (int)right;
                case TokenType.MIN:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left - (int)right;
                case TokenType.MULT:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left * (int)right;
                case TokenType.DIV:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left / (int)right;
                case TokenType.MOD:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left % (int)right;
                case TokenType.POW:
                    CheckNumberOperand(expr.Op, left, right);
                    return Math.Pow((int)left, (int)right);

                case TokenType.GREATER:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left > (int)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left >= (int)right;
                case TokenType.LESS:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left < (int)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(expr.Op, left, right);
                    return (int)left <= (int)right;
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
            }

            return null;
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.MIN:
                    CheckNumberOperand(expr.Op, right);
                    return -(int)right;
            }

            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name);
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Op.Type == TokenType.OR)
            {
                if (IsTruty(left))
                {
                    return left;
                }
            }
            else
            {
                if (!IsTruty(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.Right);
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            object callee = Evaluate(expr.Callee);

            List<object> arguments = new List<object>();
            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable function))
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != function.Arity)
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object VisitSpawnStmt(Stmt.Spawn stmt)
        {
            var x = Evaluate(stmt.X);
            var y = Evaluate(stmt.Y);

            if (!(x is int xVal) || !(y is int yVal))
            {
                throw new RuntimeError(null, "Spawn coordinates must be integers.");
            }

            if (xVal < 0 || xVal >= canvas.Size || yVal < 0 || yVal >= canvas.Size)
            {
                throw new RuntimeError(null, "Spawn coordinates are out of canvas bounds.");
            }

            Position = new Point(xVal, yVal);
            return null;
        }

        public object VisitColorStmt(Stmt.Color stmt)
        {
            CurrentColor = ParseColor(stmt.ColorName);
            return null;
        }

        public object VisitSizeStmt(Stmt.Size stmt)
        {
            object size = Evaluate(stmt.SizeValue);

            if (!(size is int sizeVal))
            {
                throw new RuntimeError(null, "Brush size must be an integer.");
            }

            if (sizeVal <= 0)
            {
                throw new RuntimeError(null, "Brush size must be positive.");
            }

            BrushSize = sizeVal % 2 == 0 ? sizeVal - 1 : sizeVal;
            return null;
        }

        public object VisitDrawLineStmt(Stmt.DrawLine stmt)
        {
            var dirX = Evaluate(stmt.DirX);
            var dirY = Evaluate(stmt.DirY);
            var distance = Evaluate(stmt.Distance);

            if (!(dirX is int dirXVal) || !(dirY is int dirYVal) || !(distance is int distanceVal))
            {
                throw new RuntimeError(null, "DrawLine parameters must be integers.");
            }

            if (Math.Abs(dirXVal) > 1 || Math.Abs(dirYVal) > 1 || (dirXVal == 0 && dirYVal == 0))
            {
                throw new RuntimeError(null, "Invalid direction for DrawLine.");
            }

            DrawLineOnCanvas(dirXVal, dirYVal, distanceVal);

            return null;
        }

        public object VisitDrawCircleStmt(Stmt.DrawCircle stmt)
        {
            var dirX = Evaluate(stmt.DirX);
            var dirY = Evaluate(stmt.DirY);
            var radius = Evaluate(stmt.Radius);


            if (!(dirX is int dirXVal) || !(dirY is int dirYVal) || !(radius is int radiusVal))
            {
                throw new RuntimeError(null, "DrawCircle parameters must be integers.");
            }

            if (radiusVal <= 0)
            {
                throw new RuntimeError(null, "Circle radius must be positive.");
            }

            DrawCircleOnCanvas(dirXVal, dirYVal, radiusVal);

            return null;
        }

        public object VisitDrawRectangleStmt(Stmt.DrawRectangle stmt)
        {
            var dirX = Evaluate(stmt.DirX);
            var dirY = Evaluate(stmt.DirY);
            var distance = Evaluate(stmt.Distance);
            var width = Evaluate(stmt.Width);
            var height = Evaluate(stmt.Height);

            if (!(dirX is int dirXVal) || !(dirY is int dirYVal) || !(distance is int distanceVal) || !(width is int widthVal) || !(height is int heightVal))
            {
                throw new RuntimeError(null, "DrawRectangle parameters must be integers.");
            }

            if (Math.Abs(dirXVal) > 1 || Math.Abs(dirYVal) > 1 || (dirXVal == 0 && dirYVal == 0))
            {
                throw new RuntimeError(null, "Invalid direction for DrawRectangle.");
            }

            DrawRectangleOnCanvas(dirXVal, dirYVal, distanceVal, widthVal, heightVal);
            return null;
        }

        public object VisitFillStmt(Stmt.Fill stmt)
        {
            FloodFill(Position.X, Position.Y, CurrentColor);
            return null;
        }

        public object VisitGotoStmt(Stmt.Goto stmt)
        {
            object condition = Evaluate(stmt.Condition);

            if (!(IsTruty(condition)))
            {
                return null;
            }
            if (!labels.TryGetValue(stmt.label.Lexeme, out int targetLine))
            {
                throw new RuntimeError(stmt.label, $"Undefined label '{stmt.label.Lexeme}'.");
            }

            currentStatement = targetLine - 1;
            return null;
        }

        public object VisitAssignmentStmt(Stmt.Assignment stmt)
        {
            object value = Evaluate(stmt.Value);
            environment.Assign(stmt.Name, value);
            return null;
        }

        public object VisitLabelStmt(Stmt.Label stmt)
        {
            return null;
        }

        private object LookUpVariable(Token token)
        {
            var name = new Expr.Variable(token);

            if (locals.TryGetValue(name, out int distance))
            {
                return environment.GetAt(distance, name.ToString());
            }
            else
            {
                return Globals.Get(token);
            }
        }

        private bool IsTruty(object obj)
        {
            if (obj == null) return false;
            if (obj is bool boolObj) return boolObj;
            if (obj is int intObj) return intObj != 0;
            return true;
        }

        private bool isEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is int) return;
            throw new RuntimeError(op, "Operand must be a number");
        }

        private void CheckNumberOperand(Token op, object left, object right)
        {
            if (left is int && right is int) return;
            throw new RuntimeError(op, "Operands must be a numbers");
        }

        public Color ParseColor(string colorName)
        {
            return colorName switch
            {
                "Red" => Color.Red,
                "Blue" => Color.Blue,
                "Green" => Color.Green,
                "Yellow" => Color.Yellow,
                "Orange" => Color.Orange,
                "Purple" => Color.Purple,
                "Black" => Color.Black,
                "White" => Color.White,
                "Transparent" => Color.Transparent,
                _ => throw new RuntimeError(null, $"Unknown color '{colorName}'.")
            };
        }

        private void DrawLineOnCanvas(int dirXVal, int dirYVal, int distanceVal)
        {
            if (Math.Abs(dirXVal) > 1 || Math.Abs(dirYVal) > 1 || (dirXVal == 0 && dirYVal == 0))
            {
                throw new RuntimeError("Invalid address for DrawLine.");
            }

            if (distanceVal <= 0)
            {
                throw new RuntimeError("Distance must be positive.");
            }

            for (var i = 0; i < distanceVal; i++)
            {
                int x = Position.X + dirXVal * i;
                int y = Position.Y + dirYVal * i;

                if (x >= 0 && x < canvas.Size && y >= 0 && y < canvas.Size)
                {
                    canvas.SetPixel(x, y, CurrentColor, BrushSize);
                }
            }

            Position = new Point(Position.X + dirXVal * distanceVal,
                                Position.Y + dirYVal * distanceVal);
        }

        private void DrawCircleOnCanvas(int dirXVal, int dirYVal, int radiusVal)
        {
            if (Math.Abs(dirXVal) > 1 || Math.Abs(dirYVal) > 1 || (dirXVal == 0 && dirYVal == 0))
            {
                throw new RuntimeError("Invalid address for DrawCircle.");
            }

            if (radiusVal <= 0)
            {
                throw new RuntimeError("Radius must be positive.");
            }

            int centerX = Position.X + dirXVal * radiusVal;
            int centerY = Position.Y + dirYVal * radiusVal;

            int x = 0;
            int y = radiusVal;
            int p = 1 - radiusVal;

            while (x <= y)
            {
                DrawCirclePoints(centerX, centerY, x, y);

                x++;

                if (p < 0)
                {
                    p += 2 * x + 1;
                }
                else
                {
                    y--;
                    p += 2 * (x - y) + 1;
                }

                Position = new Point(centerX, centerY);
            }
        }

        private void DrawCirclePoints(int centerX, int centerY, int x, int y)
        {
            SetPixelIfValid(centerX + x, centerY + y);
            SetPixelIfValid(centerX - x, centerY + y);
            SetPixelIfValid(centerX + x, centerY - y);
            SetPixelIfValid(centerX - x, centerY - y);
            SetPixelIfValid(centerX + y, centerY + x);
            SetPixelIfValid(centerX - y, centerY + x);
            SetPixelIfValid(centerX + y, centerY - x);
            SetPixelIfValid(centerX - y, centerY - x);
        }

        private void SetPixelIfValid(int x, int y)
        {
            if (x >= 0 && x < canvas.Size && y >= 0 && y < canvas.Size)
            {
                canvas.SetPixel(x, y, CurrentColor, BrushSize);
            }
        }

        private void DrawRectangleOnCanvas(int dirXVal, int dirYVal, int distanceVal, int widthVal, int heightVal)
        {
            if (Math.Abs(dirXVal) > 1 || Math.Abs(dirYVal) > 1 || (dirXVal == 0 && dirYVal == 0))
            {
                throw new RuntimeError("Invalid address for DrawRectangle.");
            }

            if (distanceVal <= 0 || widthVal <= 0 || heightVal <= 0)
            {
                throw new RuntimeError("Distance, width, and height must be positive.");
            }

            int centerX = Position.X + dirXVal * distanceVal;
            int centerY = Position.Y + dirYVal * distanceVal;

            int startX = centerX - widthVal / 2;
            int endX = centerX + widthVal / 2;
            int startY = centerY - heightVal / 2;
            int endY = centerY + heightVal / 2;

            for (var x = startX; x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    if (x >= 0 && x < canvas.Size && y >= 0 && y < canvas.Size)
                    {
                        canvas.SetPixel(x, y, CurrentColor, BrushSize);
                    }
                }
            }

            Position = new Point(centerX, centerY);
        }

        private void FloodFill(int x, int y, Color newColor)
        {
            Color targetColor = canvas.GetPixel(x, y);

            if (targetColor == newColor || targetColor == Color.Transparent)
            {
                return;
            }

            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(x, y));

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                int currentX = current.X;
                int currentY = current.Y;

                if (currentX >= 0 && currentX < canvas.Size && currentY >= 0 && currentY < canvas.Size &&
                    canvas.GetPixel(currentX, currentY) == targetColor)
                {
                    canvas.SetPixel(currentX, currentY, newColor, BrushSize);

                    queue.Enqueue(new Point(currentX + 1, currentY));
                    queue.Enqueue(new Point(currentX - 1, currentY));
                    queue.Enqueue(new Point(currentX, currentY + 1));
                    queue.Enqueue(new Point(currentX, currentY - 1));
                }
            }
        }

        internal void Resolve(Expr expr, int depth)
        {
            locals[expr] = depth;
        }

    }

    internal interface ICallable
    {
        int Arity { get; }
        object Call(Interpreter interpreter, List<object> arguments);
    }

    public class GetActualX : ICallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return interpreter.Position.X;
        }

    }

    public class GetActualY : ICallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return interpreter.Position.Y;
        }

    }

    public class GetCanvasSize : ICallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return interpreter.canvas.Size;
        }

    }

    public class GetColorCount : ICallable
    {
        public int Arity => 5;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string colorName && arguments[1] is int x1 && arguments[2] is int y1 && arguments[3] is int x2 && arguments[4] is int y2)
            {
                Color color = interpreter.ParseColor(colorName);

                if (x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0 ||
                    x1 >= interpreter.canvas.Size || x2 >= interpreter.canvas.Size ||
                    y1 >= interpreter.canvas.Size || y2 >= interpreter.canvas.Size)
                {
                    return 0;
                }

                return interpreter.canvas.CountColor(color, x1, y1, x2, y2);
            }
            throw new RuntimeError(null, "Expected a color name and four integers as arguments.");
        }

    }

    public class IsBrushColor : ICallable
    {
        public int Arity => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string colorName)
            {
                Color color = interpreter.ParseColor(colorName);
                return interpreter.CurrentColor == color ? 1 : 0;
            }
            throw new RuntimeError(null, "Expected a color name as argument.");
        }

    }

    public class IsBrushSize : ICallable
    {
        public int Arity => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is int size)
            {
                return interpreter.BrushSize == size;
            }
            throw new RuntimeError(null, "Expected an integer as argument.");
        }

    }

    public class IsCanvasColor : ICallable
    {
        public int Arity => 3;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string colorName && arguments[1] is int vertical && arguments[2] is int horizontal)
            {
                Color color = interpreter.ParseColor(colorName);

                int targetX = interpreter.Position.X + horizontal;
                int targetY = interpreter.Position.Y + vertical;

                if (targetX < 0 || targetX >= interpreter.canvas.Size || targetY < 0 || targetY >= interpreter.canvas.Size)
                {
                    return 0;
                }

                return interpreter.canvas.GetPixel(targetX, targetY) == color ? 1 : 0;
            }
            throw new RuntimeError(null, "Expected a color name and two integers as arguments.");
        }

    }
}