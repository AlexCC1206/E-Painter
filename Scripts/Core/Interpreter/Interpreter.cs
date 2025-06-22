using System;
using System.Collections.Generic;
using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    /// <summary>
    /// Clase principal que interpreta y ejecuta el código del lenguaje E-Painter.
    /// </summary>
    public class Interpreter
    {
    /// <summary>
    /// Diccionario que almacena las variables definidas durante la ejecución.
    /// </summary>
        private Dictionary<string, object> Variables = new Dictionary<string, object>();
        
    /// <summary>
    /// Diccionario que mapea nombres de etiquetas a índices en la lista de sentencias.
    /// </summary>
        private Dictionary<string, int> Labels = new Dictionary<string, int>();

        /// <summary>
        /// Estado actual del intérprete que incluye posición, color y tamaño del pincel.
        /// </summary>
        private EPainterState state;
        
        /// <summary>
        /// El lienzo donde se realizan las operaciones de dibujo.
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// La lista de sentencias a ejecutar.
        /// </summary>
        private List<Stmt> statements;

        /// <summary>
        /// Inicia la interpretación de un programa E-Painter.
        /// </summary>
        /// <param name="canvas">El lienzo donde se realizarán los dibujos.</param>
        /// <param name="stmts">La lista de sentencias a interpretar.</param>
        public void Interpret(Canvas canvas, List<Stmt> stmts)
        {
            this.canvas = canvas;
            statements = stmts;
            
            ValidateSpawnIsFirstAndUnique();
            
            state = new EPainterState(0,0);
            InitializeLabels();
            ExecuteAll();
        }
        
        /// <summary>
        /// Valida que haya exactamente un comando Spawn al inicio del programa.
        /// Las etiquetas pueden aparecer antes del Spawn, pero no otras instrucciones.
        /// </summary>
        /// <exception cref="RuntimeError">Se lanza si no se cumple la validación.</exception>
        private void ValidateSpawnIsFirstAndUnique()
        {
            bool foundSpawn = false;
            int firstSpawnIndex = -1;

            if (!foundSpawn)
            {
                var error = new RuntimeError("The program must start with a Spawn command.");
                ErrorReporter.RuntimeError(error);
                throw error;
            }
            
            for (int i = 0; i < firstSpawnIndex; i++)
            {
                if (!(statements[i] is Label))
                {
                    var error = new RuntimeError("The Spawn command must be the first command in the program (labels can appear before).");
                    ErrorReporter.RuntimeError(error);
                    throw error;
                }
            }
            
            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i] is Spawn)
                {
                    if (!foundSpawn)
                    {
                        foundSpawn = true;
                        firstSpawnIndex = i;
                    }
                    else
                    {
                        var error = new RuntimeError($"Multiple Spawn commands found. There should be only one Spawn command at the beginning of the program.");
                        ErrorReporter.RuntimeError(error);
                        throw error;
                    }
                }
            }
        }

        /// <summary>
        /// Inicializa el diccionario de etiquetas, escaneando todas las sentencias Label.
        /// </summary>
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

        /// <summary>
        /// Ejecuta todas las sentencias en secuencia, manejando los saltos entre etiquetas.
        /// </summary>
        /// <exception cref="RuntimeError">Se lanza si se detecta un bucle infinito o una etiqueta no encontrada.</exception>
        private void ExecuteAll()
        {
            int currentStatement = 0;
            
            Dictionary<string, int> labelJumpCounts = new Dictionary<string, int>();
            
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
                    
                    if (!labelJumpCounts.ContainsKey(gotoEx.Label))
                    {
                        labelJumpCounts[gotoEx.Label] = 1;
                    }
                    else
                    {
                        labelJumpCounts[gotoEx.Label]++;
                        
                        if (labelJumpCounts[gotoEx.Label] > MAX_JUMPS_TO_SAME_LABEL)
                        {
                            var infiniteLoopError = new RuntimeError($"Possible infinite loop detected: jumped to label '{gotoEx.Label}' more than {MAX_JUMPS_TO_SAME_LABEL} times.");
                            ErrorReporter.RuntimeError(infiniteLoopError);
                            throw infiniteLoopError;
                        }
                    }

                    currentStatement = targetLine;
                    continue;
                }
            }
        }

    /// <summary>
    /// Ejecuta una sentencia individual.
    /// </summary>
    /// <param name="stmt">La sentencia a ejecutar.</param>
        public void Execute(Stmt stmt)
        {
            stmt.Accept(new StmtVisitor(this));
        }

    /// <summary>
    /// Evalúa una expresión y devuelve su valor.
    /// </summary>
    /// <param name="expr">La expresión a evaluar.</param>
    /// <returns>El valor resultante de la expresión.</returns>
        public object Evaluate(Expr expr)
        {
            return expr.Accept(new ExprVisitor(this));
        }

    /// <summary>
    /// Obtiene el valor de una variable del entorno.
    /// </summary>
    /// <param name="name">El nombre de la variable a obtener.</param>
    /// <returns>El valor de la variable.</returns>
    /// <exception cref="RuntimeError">Se lanza si la variable no está definida.</exception>
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

    /// <summary>
    /// Establece el valor de una variable en el entorno.
    /// </summary>
    /// <param name="name">El nombre de la variable a establecer.</param>
    /// <param name="value">El valor a asignar a la variable.</param>
        public void SetVariable(string name, object value)
        {
            Variables[name] = value;
        }

    /// <summary>
    /// Obtiene el tamaño del lienzo.
    /// </summary>
    /// <returns>El tamaño del lienzo.</returns>
        public int GetCanvasSize()
        {
            return canvas.Size;
        }

    /// <summary>
    /// Obtiene la posición actual en X del "pincel".
    /// </summary>
    /// <returns>La coordenada X actual.</returns>
        public int GetActualX()
        {
            return state.X;
        }

    /// <summary>
    /// Obtiene la posición actual en Y del "pincel".
    /// </summary>
    /// <returns>La coordenada Y actual.</returns>
        public int GetActualY()
        {
            return state.Y;
        }

    /// <summary>
    /// Verifica si el color del pincel es igual al color dado.
    /// </summary>
    /// <param name="color">El color a comparar.</param>
    /// <returns>1 si el color del pincel es igual, 0 en caso contrario.</returns>
        public int IsBrushColor(string color)
        {
            return state.BrushColor == color ? 1 : 0;
        }

    /// <summary>
    /// Verifica si el tamaño del pincel es igual al tamaño dado.
    /// </summary>
    /// <param name="size">El tamaño a comparar.</param>
    /// <returns>1 si el tamaño del pincel es igual, 0 en caso contrario.</returns>
        public int IsBrushSize(int size)
        {
            return state.BrushSize == size ? 1 : 0;
        }

    /// <summary>
    /// Verifica si el color en la posición dada (con offset) es igual al color dado.
    /// </summary>
    /// <param name="color">El color a comparar.</param>
    /// <param name="dx">Desplazamiento en X.</param>
    /// <param name="dy">Desplazamiento en Y.</param>
    /// <returns>1 si el color es igual, 0 en caso contrario.</returns>
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

    /// <summary>
    /// Cuenta la cantidad de píxeles de un color específico en un área rectangular.
    /// </summary>
    /// <param name="color">El color a contar.</param>
    /// <param name="x1">Esquina superior izquierda X.</param>
    /// <param name="y1">Esquina superior izquierda Y.</param>
    /// <param name="x2">Esquina inferior derecha X.</param>
    /// <param name="y2">Esquina inferior derecha Y.</param>
    /// <returns>La cantidad de píxeles del color en el área especificada.</returns>
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

    /// <summary>
    /// Crea una nueva "instancia" del lápiz en la posición dada.
    /// </summary>
    /// <param name="x">Coordenada X de la nueva posición.</param>
    /// <param name="y">Coordenada Y de la nueva posición.</param>
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

    /// <summary>
    /// Establece el color del pincel.
    /// </summary>
    /// <param name="color">El color a establecer.</param>
        public void SetBrushColor(string color)
        {
            state.BrushColor = color;
        }

    /// <summary>
    /// Establece el tamaño del pincel.
    /// </summary>
    /// <param name="size">El tamaño a establecer. Se ajusta a un valor impar si es necesario.</param>
        public void SetBrushSize(int size)
        {
            if (size <= 0)
            {
                var sizeError = new RuntimeError($"Brush size must be positive. Value provided: {size}");
                ErrorReporter.RuntimeError(sizeError);
                throw sizeError;
            }
                
            state.BrushSize = size % 2 == 0 ? size - 1 : size;
        }

    /// <summary>
    /// Dibuja una línea en la dirección y distancia especificadas.
    /// </summary>
    /// <param name="dirX">Dirección en X.</param>
    /// <param name="dirY">Dirección en Y.</param>
    /// <param name="distance">Distancia a dibujar.</param>
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
            
            state.X = centerX + dirX * distance;
            state.Y = centerY + dirY * distance;
        }

    /// <summary>
    /// Dibuja un círculo en la dirección y radio especificados.
    /// </summary>
    /// <param name="dirX">Dirección en X para el centro del círculo.</param>
    /// <param name="dirY">Dirección en Y para el centro del círculo.</param>
    /// <param name="radius">Radio del círculo.</param>
        public void DrawCircle(int dirX, int dirY, int radius)
        {
            int centerX = state.X;
            int centerY = state.Y;

            int circleCenterX = centerX + dirX * radius;
            int circleCenterY = centerY + dirY * radius;
            
            if (!canvas.IsValidPosition(circleCenterX, circleCenterY))
            {
                var circleError = new RuntimeError($"Circle center out of bounds at ({circleCenterX}, {circleCenterY})");
                ErrorReporter.RuntimeError(circleError);
                throw circleError;
            }
            
            if (!canvas.IsValidPosition(circleCenterX + radius, circleCenterY) ||
                !canvas.IsValidPosition(circleCenterX - radius, circleCenterY) ||
                !canvas.IsValidPosition(circleCenterX, circleCenterY + radius) ||
                !canvas.IsValidPosition(circleCenterX, circleCenterY - radius))
            {

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

    /// <summary>
    /// Dibuja los puntos del círculo usando el algoritmo de simetría de 8 vías.
    /// </summary>
    /// <param name="circleCenterX">Coordenada X del centro del círculo.</param>
    /// <param name="circleCenterY">Coordenada Y del centro del círculo.</param>
    /// <param name="x">Desplazamiento X desde el centro del círculo.</param>
    /// <param name="y">Desplazamiento Y desde el centro del círculo.</param>
    /// <param name="brushSize">Tamaño del pincel (mitad).</param>
        private void DrawCirclePoints(int circleCenterX, int circleCenterY, int x, int y, int brushSize)
        {
            Plot8Points(circleCenterX, circleCenterY, x, y, brushSize);
        }

    /// <summary>
    /// Dibuja los 8 puntos simétricos de un círculo.
    /// </summary>
    /// <param name="circleCenterX">Coordenada X del centro del círculo.</param>
    /// <param name="circleCenterY">Coordenada Y del centro del círculo.</param>
    /// <param name="x">Desplazamiento X desde el centro del círculo.</param>
    /// <param name="y">Desplazamiento Y desde el centro del círculo.</param>
    /// <param name="brushSize">Tamaño del pincel (mitad).</param>
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

    /// <summary>
    /// Dibuja un área de píxeles utilizando el tamaño de pincel actual.
    /// </summary>
    /// <param name="x">Coordenada X del centro del área a dibujar.</param>
    /// <param name="y">Coordenada Y del centro del área a dibujar.</param>
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

    /// <summary>
    /// Dibuja un rectángulo en la dirección, distancia y dimensiones especificadas.
    /// </summary>
    /// <param name="dirX">Dirección en X.</param>
    /// <param name="dirY">Dirección en Y.</param>
    /// <param name="distance">Distancia a dibujar.</param>
    /// <param name="width">Ancho del rectángulo.</param>
    /// <param name="height">Altura del rectángulo.</param>
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

        /// <summary>
        /// Rellena el área conectada al píxel actual con el color del pincel.
        /// </summary>
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