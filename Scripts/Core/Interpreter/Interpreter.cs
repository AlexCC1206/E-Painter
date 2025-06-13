using System;
using System.Collections.Generic;
using System.Drawing;
using static EPainter.Core.Expr;
using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    public class Interpreter
    {
        private Dictionary<string, object> Variables = new Dictionary<string, object>();
        private Dictionary<string, int> Labels = new Dictionary<string, int>();

        private EPainterState state;
        public Canvas canvas;

        private List<Stmt> statements;

        public Interpreter(Canvas canvas, List<Stmt> stmts)
        {
            this.canvas = canvas;
            statements = stmts;
            InitializeLabels();
            ExecuteAll();
        }


        private void InitializeLabels()
        {
            for (int i = 0; i < statements.Count; i++)
            {
                var stmt = statements[i];
                if (stmt is Stmt.Label labelStmt)
                {
                    Labels[labelStmt.Name] = i;
                }
            }
        }

        private void ExecuteAll()
        {
            int currentStatement = 0;
            while (currentStatement < statements.Count)
            {
                var stmt = statements[currentStatement];

                if (stmt is Stmt.Label)
                {
                    currentStatement++;
                    continue;
                }

                if (stmt is Stmt.Goto gotoStmt)
                {
                    var condition = Evaluate(gotoStmt.Condition);
                    if ((bool)condition)
                    {
                        if (!Labels.TryGetValue(gotoStmt.LabelName, out int targetLine))
                        {
                            throw new RuntimeError($"Label '{gotoStmt.LabelName}' not found.");
                        }

                        currentStatement = targetLine;
                        continue;
                    }
                }

                Execute(stmt);

                currentStatement++;
            }

        }

        public void Execute(Stmt stmt)
        {
            stmt.Accept(new IStmtVisitor(this));
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(new IExprVisitor());
        }

        public object GetVariable(string name)
        {
            if (Variables.TryGetValue(name, out var value))
            {
                return value;
            }
            throw new RuntimeError($"Undefined variable '{name}'.");
        }

        public void SetVariable(string name, object value)
        {
            Variables[name] = value;
        }

        public int GetCanvasSize()
        {
            return canvas.Size;
        }

        public int GetActualX()
        {
            return state.X;
        }

        public int GetActualY()
        {
            return state.Y;
        }

        public int IsBrushSize(int size)
        {
            return state.BrushSize == size ? 1 : 0;
        }

        public int IsBrushColor(string color)
        {
            return state.BrushColor == color ? 1 : 0;
        }

        public int IsCanvasColor(string color, int dx, int dy)
        {
            int x = state.X + dx;
            int y = state.Y + dy;

            if (!canvas.IsValidPosition(x, y))
            {
                return 0;
            }

            return canvas.GetPixel(x, y) == color ? 1 : 0;
        }

        public int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            int count = 0;
            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);
            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (canvas.IsValidPosition(x, y) && canvas.GetPixel(x, y) == color)
                        count++;
                }
            }

            return count;
        }

        public void Spawn(int x, int y)
        {
            if (canvas.IsValidPosition(x, y))
            {
                throw new RuntimeError($"Spawn position ({x}, {y}) out of bounds.");
            }
            state = new EPainterState(x, y);
        }

        public void SetBrushColor(string color)
        {
            state.BrushColor = color;
        }

        public void SetBrushSize(int size)
        {
            state.BrushSize = size % 2 == 0 ? size - 1 : size;
        }

        public void DrawLine(int dirX, int dirY, int distance)
        {
            // Implementar dibujo de línea con tamaño de pincel
            // Usa `_state.X`, `_state.Y`, `_state.BrushSize`
            // Dibuja en `_canvas.SetPixel(...)`
        }

        public void DrawCircle(int dirX, int dirY, int radius)
        {
            // Implementar dibujo de círculo
        }

        public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            // Implementar rectángulo
        }

        public void Fill()
        {
            // Implementar flood fill desde posición actual
        }
        
        /*
                public object VisitBinaryExpr(Binary expr)
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

            }*/
    }
}