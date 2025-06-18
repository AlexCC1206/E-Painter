using System;
using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    /// <summary>
    /// Implementa el patrón Visitor para interpretar las sentencias del lenguaje E-Painter.
    /// </summary>
    public class StmtVisitor : IStmtVisitor<object>
    {
        /// <summary>
        /// La instancia del intérprete que utiliza este visitante.
        /// </summary>
        private Interpreter interpreter;

        /// <summary>
        /// Inicializa una nueva instancia de la clase StmtVisitor.
        /// </summary>
        /// <param name="interpreter">La instancia del intérprete a utilizar.</param>
        public StmtVisitor(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        /// <summary>
        /// Procesa una sentencia de asignación de variable.
        /// </summary>
        /// <param name="stmt">La sentencia de asignación a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitAssignment(Assignment stmt)
        {
            var value = interpreter.Evaluate(stmt.Value);
            interpreter.SetVariable(stmt.Name, value);
            return null;
        }
        
        /// <summary>
        /// Procesa una sentencia Spawn que establece la posición inicial.
        /// </summary>
        /// <param name="stmt">La sentencia Spawn a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitSpawn(Spawn stmt)
        {
            int x = Convert.ToInt32(interpreter.Evaluate(stmt.X));
            int y = Convert.ToInt32(interpreter.Evaluate(stmt.Y));
            interpreter.Spawn(x, y);
            return null;
        }

        /// <summary>
        /// Procesa una sentencia Color que cambia el color del pincel.
        /// </summary>
        /// <param name="stmt">La sentencia Color a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitColor(Color stmt)
        {
            string color = (string)interpreter.Evaluate(stmt.ColorName);
            interpreter.SetBrushColor(color);
            return null;
        }
        
        /// <summary>
        /// Procesa una sentencia Size que cambia el tamaño del pincel.
        /// </summary>
        /// <param name="stmt">La sentencia Size a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitSize(Size stmt)
        {
            int size = Convert.ToInt32(interpreter.Evaluate(stmt.SizeValue));
            interpreter.SetBrushSize(size);
            return null;
        }

        /// <summary>
        /// Procesa una sentencia DrawLine que dibuja una línea.
        /// </summary>
        /// <param name="stmt">La sentencia DrawLine a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitDrawLine(DrawLine stmt)
        {
            int dx = Convert.ToInt32(interpreter.Evaluate(stmt.DirX));
            int dy = Convert.ToInt32(interpreter.Evaluate(stmt.DirY));
            int dist = Convert.ToInt32(interpreter.Evaluate(stmt.Distance));
            interpreter.DrawLine(dx, dy, dist);
            return null;
        }

        /// <summary>
        /// Procesa una sentencia DrawCircle que dibuja un círculo.
        /// </summary>
        /// <param name="stmt">La sentencia DrawCircle a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitDrawCircle(DrawCircle stmt)
        {
            int dx = Convert.ToInt32(interpreter.Evaluate(stmt.DirX));
            int dy = Convert.ToInt32(interpreter.Evaluate(stmt.DirY));
            int radius = Convert.ToInt32(interpreter.Evaluate(stmt.Radius));
            interpreter.DrawCircle(dx, dy, radius);
            return null;
        }
        
        /// <summary>
        /// Procesa una sentencia DrawRectangle que dibuja un rectángulo.
        /// </summary>
        /// <param name="stmt">La sentencia DrawRectangle a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitDrawRectangle(DrawRectangle stmt)
        {
            int dx = Convert.ToInt32(interpreter.Evaluate(stmt.DirX));
            int dy = Convert.ToInt32(interpreter.Evaluate(stmt.DirY));
            int dist = Convert.ToInt32(interpreter.Evaluate(stmt.Distance));
            int width = Convert.ToInt32(interpreter.Evaluate(stmt.Width));
            int height = Convert.ToInt32(interpreter.Evaluate(stmt.Height));
            interpreter.DrawRectangle(dx, dy, dist, width, height);
            return null;
        }

        /// <summary>
        /// Procesa una sentencia Fill que rellena una forma cerrada.
        /// </summary>
        /// <param name="stmt">La sentencia Fill a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitFill(Fill stmt)
        {
            interpreter.Fill();
            return null;
        }
        
        /// <summary>
        /// Procesa una sentencia GoTo que salta a una etiqueta.
        /// </summary>
        /// <param name="stmt">La sentencia GoTo a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        /// <exception cref="GotoException">Lanza una excepción para indicar un salto a una etiqueta.</exception>
        public object VisitGoto(Goto stmt)
        {
            bool condition = (bool)interpreter.Evaluate(stmt.Condition);
            if (condition)
            {
                throw new GotoException(stmt.LabelName);
            }
            return null;
        }

        /// <summary>
        /// Procesa una sentencia Label que define una etiqueta.
        /// </summary>
        /// <param name="stmt">La sentencia Label a procesar.</param>
        /// <returns>Null (las sentencias no devuelven valores).</returns>
        public object VisitLabel(Label stmt)
        {
            return null;
        }
    }
}