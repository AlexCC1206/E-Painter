using System;
using System.Collections.Generic;
using static EPainter.Core.Expr;
using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    public class Interpreter
    {
        private Dictionary<string, object> Variables = new Dictionary<string, object>();

        /// <summary>
        /// Diccionario que mapea nombres de etiquetas a índices en la lista de sentencias.
        /// </summary>
        public Dictionary<string, int> Labels = new Dictionary<string, int>();

        private EPainterState state;
        public Canvas canvas;

        private List<Stmt> statements;

        public void Interpret(Canvas canvas, List<Stmt> stmts)
        {
            this.canvas = canvas;
            statements = stmts;
            
            // Validar que Spawn sea el primer comando y único en el código
            ValidateSpawnIsFirstAndUnique();
            
            state = new EPainterState(0,0);
            InitializeLabels();
            ExecuteAll();
        }
        
        private void ValidateSpawnIsFirstAndUnique()
        {
            bool foundSpawn = false;
            int firstSpawnIndex = -1;
            
            // Buscar los comandos Spawn en el código
            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i] is Stmt.Spawn)
                {
                    if (!foundSpawn)
                    {
                        foundSpawn = true;
                        firstSpawnIndex = i;
                    }
                    else
                    {
                        var error = new RuntimeError($"Múltiples comandos Spawn encontrados. Solo debe haber un comando Spawn al inicio del programa.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                }
            }
            
            if (!foundSpawn)
            {
                var error = new RuntimeError("El programa debe comenzar con un comando Spawn.");
                ErrorReporter.RuntimeError(error);
                throw error;
            }
            
            for (int i = 0; i < firstSpawnIndex; i++)
            {
                if (!(statements[i] is Stmt.Label))
                {
                    var error = new RuntimeError("El comando Spawn debe ser el primer comando del programa (las etiquetas pueden aparecer antes).");
                    ErrorReporter.RuntimeError(error);
                    throw error;
                }
            }
        }

        private void InitializeLabels()
        {
            for (int i = 0; i < statements.Count; i++)
            {
                var stmt = statements[i];
                if (stmt is Label labelStmt)
                {
                    Labels[labelStmt.Name] = i;
                }
            }
        }

        private void ExecuteAll()
        {
            int currentStatement = 0;
            // Diccionario para contar la cantidad de veces que se salta a cada etiqueta
            Dictionary<string, int> labelJumpCounts = new Dictionary<string, int>();
            // Límite máximo de saltos a la misma etiqueta para detectar ciclos infinitos
            const int MAX_JUMPS_TO_SAME_LABEL = 1000;
            
            while (currentStatement < statements.Count)
            {
                var stmt = statements[currentStatement];

                if (stmt is Label)
                {
                    currentStatement++;
                    continue;
                }

                try 
                {
                    Execute(stmt);
                    currentStatement++;
                }
                catch (GotoException gotoEx)
                {
                    if (!Labels.TryGetValue(gotoEx.Label, out int targetLine))
                    {
                        var error = new RuntimeError($"Label '{gotoEx.Label}' not found.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                    
                    // Incrementar el contador de saltos para esta etiqueta
                    if (!labelJumpCounts.ContainsKey(gotoEx.Label))
                    {
                        labelJumpCounts[gotoEx.Label] = 1;
                    }
                    else
                    {
                        labelJumpCounts[gotoEx.Label]++;
                        
                        // Verificar si se excedió el límite de saltos
                        if (labelJumpCounts[gotoEx.Label] > MAX_JUMPS_TO_SAME_LABEL)
                        {
                            var infiniteLoopError = new RuntimeError($"Posible ciclo infinito detectado: se ha saltado a la etiqueta '{gotoEx.Label}' más de {MAX_JUMPS_TO_SAME_LABEL} veces.");
                            ErrorReporter.RuntimeError(infiniteLoopError);
                            throw infiniteLoopError;
                        }
                    }

                    currentStatement = targetLine;
                    continue;
                }
            }
        }

        public void Execute(Stmt stmt)
        {
            stmt.Accept(new StmtVisitor(this));
        }

        public object Evaluate(Expr expr)
        {
            return expr.Accept(new ExprVisitor(this));
        }

        public object GetVariable(string name)
        {
            if (Variables.TryGetValue(name, out var value))
            {
                return value;
            }
            var varError = new RuntimeError($"Undefined variable '{name}'.");
            ErrorReporter.RuntimeError(varError);
            throw varError;
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
            if (!canvas.IsValidPosition(x, y))
            {
                var spawnError = new RuntimeError($"Spawn position ({x}, {y}) out of bounds.");
                ErrorReporter.RuntimeError(spawnError);
                throw spawnError;
            }
            state = new EPainterState(x, y);
        }

        public void SetBrushColor(string color)
        {
            state.BrushColor = color;
        }

        public void SetBrushSize(int size)
        {
            if (size <= 0)
            {
                var sizeError = new RuntimeError($"El tamaño del pincel debe ser positivo. Valor proporcionado: {size}");
                ErrorReporter.RuntimeError(sizeError);
                throw sizeError;
            }
                
            state.BrushSize = size % 2 == 0 ? size - 1 : size;
        }

        public void DrawLine(int dirX, int dirY, int distance)
        {
            int centerX = state.X;
            int centerY = state.Y;
            int brushSize = state.BrushSize / 2;

            if ((dirX == 0 && dirY == 0) || (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1))
            {
                var dirError = new RuntimeError("Invalid direction for DrawLine");
                ErrorReporter.RuntimeError(dirError);
                throw dirError;
            }

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
            
            // Actualizar la posición actual
            state.X = centerX + dirX * distance;
            state.Y = centerY + dirY * distance;
        }

        public void DrawCircle(int dirX, int dirY, int radius)
        {
            int centerX = state.X;
            int centerY = state.Y;

            int circleCenterX = centerX + dirX * radius;
            int circleCenterY = centerY + dirY * radius;
            
            // Comprobar si el círculo está dentro de los límites del canvas
            if (!canvas.IsValidPosition(circleCenterX, circleCenterY))
            {
                var circleError = new RuntimeError($"Circle center out of bounds at ({circleCenterX}, {circleCenterY})");
                ErrorReporter.RuntimeError(circleError);
                throw circleError;
            }
            
            // También verificar que el radio no hace que el círculo se salga del canvas
            if (!canvas.IsValidPosition(circleCenterX + radius, circleCenterY) ||
                !canvas.IsValidPosition(circleCenterX - radius, circleCenterY) ||
                !canvas.IsValidPosition(circleCenterX, circleCenterY + radius) ||
                !canvas.IsValidPosition(circleCenterX, circleCenterY - radius))
            {
                // No lanzar error, solo ajustar el radio para que quepa en el canvas
                int maxRadius = Math.Min(
                    Math.Min(canvas.Size - 1 - circleCenterX, circleCenterX),
                    Math.Min(canvas.Size - 1 - circleCenterY, circleCenterY));
                
                if (maxRadius <= 0)
                {
                    var radiusError = new RuntimeError($"Cannot draw circle, radius too large for canvas");
                    ErrorReporter.RuntimeError(radiusError);
                    throw radiusError;
                }
                
                radius = Math.Min(radius, maxRadius);
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
                var rectError = new RuntimeError("Rectangle center out of bounds");
                ErrorReporter.RuntimeError(rectError);
                throw rectError;
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