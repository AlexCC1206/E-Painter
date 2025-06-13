using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    public class StmtVisitor : IStmtVisitor<object>
    {
        public Interpreter interpreter;

        public StmtVisitor(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public object VisitAssignment(Assignment stmt)
        {
            var value = interpreter.Execute(stmt.Name);
        }

        public object VisitColor(Color stmt)
        {
            CurrentColor = ParseColor(stmt.ColorName);
            return null;
        }

        public object VisitDrawCircle(DrawCircle stmt)
        {
            int dx = (int)interpreter.Evaluate(stmt.DirX);
            int dy = (int)interpreter.Evaluate(stmt.DirY);
            int radius = (int)interpreter.Evaluate(stmt.Radius);
            interpreter.DrawCircle(dx, dy, radius);
            return null;
        }

        public object VisitDrawLine(DrawLine stmt)
        {
            int dx = (int)interpreter.Evaluate(stmt.DirX);
            int dy = (int)interpreter.Evaluate(stmt.DirY);
            int dist = (int)interpreter.Evaluate(stmt.Distance);
            interpreter.DrawLine(dx, dy, dist);
            return null;
        }

        public object VisitDrawRectangle(DrawRectangle stmt)
        {
            int dx = (int)interpreter.Evaluate(stmt.DirX);
            int dy = (int)interpreter.Evaluate(stmt.DirY);
            int dist = (int)interpreter.Evaluate(stmt.Distance);
            int width = (int)interpreter.Evaluate(stmt.Width);
            int height = (int)interpreter.Evaluate(stmt.Height);
            interpreter.DrawRectangle(dx, dy, dist, width, height);
            return null;
        }

        public object VisitFill(Fill stmt)
        {
            interpreter.Fill();
            return null;
        }

        public object VisitGoto(Goto stmt)
        {
            return null;
        }

        public object VisitLabel(Label stmt)
        {
            return null;
        }

        public object VisitSize(Size stmt)
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

        public object VisitSpawn(Spawn stmt)
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
    }
}