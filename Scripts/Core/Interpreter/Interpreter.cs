using System;
using System.Collections.Generic;
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

        public void Interpret(Canvas canvas, List<Stmt> stmts)
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
            stmt.Accept(new StmtVisitor(this));
        }

        public object Evaluate(Expr expr)
        {
            return expr.Accept(new ExprVisitor());
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

        public int IsBrushColor(string color)
        {
            return state.BrushColor == color ? 1 : 0;
        }

        public int IsBrushSize(int size)
        {
            return state.BrushSize == size ? 1 : 0;
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
            int centerX = state.X;
            int centerY = state.Y;
            int brushSize = state.BrushSize / 2;

            if ((dirX == 0 && dirY == 0) || (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1))
                throw new RuntimeError("Invalid direction for DrawLine");

            for (var i = 0; i <= distance; i++)
            {
                int x = centerX + dirX * i;
                int y = centerY + dirY * i;

                for (int dx = -brushSize; dx <= brushSize; dx++)
                {
                    for (int dy = -brushSize; dy <= brushSize; dy++)
                    {
                        canvas.SetPixel(x + dx, y + dy, state.BrushColor);
                    }
                }
            }

            state.X += dirX * distance;
            state.Y += dirY * distance;
        }

        public void DrawCircle(int dirX, int dirY, int radius)
        {
            int centerX = state.X;
            int centerY = state.Y;

            int circleCenterX = centerX + dirX * radius;
            int circleCenterY = centerY + dirY * radius;

            if (!canvas.IsValidPosition(circleCenterX, circleCenterY))
            {
                throw new RuntimeError("Circle center out of bounds");
            }

            int brushSize = state.BrushSize / 2;

            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            while (x <= y)
            {
                DrawCirclePoints(circleCenterX, circleCenterY, x, y, brushSize);

                x++;
                if (d < 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    y--;
                    d += 4 * (x - y) + 10;
                }
            }

            state.X = circleCenterX;
            state.Y = circleCenterY;
        }



        private void DrawCirclePoints(int circleCenterX, int circleCenterY, int x, int y, int brushSize)
        {
            Plot8Points(circleCenterX, circleCenterY, x, y, brushSize);
        }

        private void Plot8Points(int circleCenterX, int circleCenterY, int x, int y, int brushSize)
        {
            DrawPixelArea(circleCenterX + x, circleCenterY + y);
            DrawPixelArea(circleCenterX - x, circleCenterY + y);
            DrawPixelArea(circleCenterX + x, circleCenterY - y);
            DrawPixelArea(circleCenterX - x, circleCenterY - y);
            DrawPixelArea(circleCenterX + y, circleCenterY + x);
            DrawPixelArea(circleCenterX - y, circleCenterY + x);
            DrawPixelArea(circleCenterX + y, circleCenterY - x);
            DrawPixelArea(circleCenterX - y, circleCenterY - x);
        }


        private void DrawPixelArea(int x, int y)
        {
            int halfBrush = state.BrushSize / 2;

            for (int dx = -halfBrush; dx <= halfBrush; dx++)
            {
                for (int dy = -halfBrush; dy <= halfBrush; dy++)
                {
                    canvas.SetPixel(x + dx, y + dy, state.BrushColor);
                }
            }
        }

        public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            int centerX = state.X;
            int centerY = state.Y;

            int rectCenterX = centerX + dirX * distance;
            int rectCenterY = centerY + dirY * distance;

            if (!canvas.IsValidPosition(rectCenterX, rectCenterY))
            {
                throw new RuntimeError("Rectangle center out of bounds");
            }

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            int brushSize = state.BrushSize / 2;

            for (int dx = -halfWidth; dx <= halfWidth; dx++)
            {
                for (int dy = -halfHeight; dy <= halfHeight; dy++)
                {
                    if (dx == -halfWidth || dx == halfWidth ||
                        dy == -halfHeight || dy == halfHeight)
                    {
                        for (int bdx = -brushSize; bdx <= brushSize; bdx++)
                        {
                            for (int bdy = -brushSize; bdy <= brushSize; bdy++)
                            {
                                canvas.SetPixel(rectCenterX + dx + bdx, rectCenterY + dy + bdy, state.BrushColor);
                            }
                        }
                    }
                }
            }

            state.X = rectCenterX;
            state.Y = rectCenterY;
        }

        public void Fill()
        {
            string targetColor = canvas.GetPixel(state.X, state.Y);
            if (targetColor == null || targetColor == state.BrushColor)
                return;

            var queue = new Queue<(int, int)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((state.X, state.Y));
            visited.Add((state.X, state.Y));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                canvas.SetPixel(x, y, state.BrushColor);

                foreach (var (dx, dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (canvas.IsValidPosition(nx, ny) &&
                        canvas.GetPixel(nx, ny) == targetColor &&
                        !visited.Contains((nx, ny)))
                    {
                        visited.Add((nx, ny));
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }
    }
}