using System;
using static EPainter.Core.Stmt;

namespace EPainter.Core
{
    public class StmtVisitor : IStmtVisitor<object>
    {
        private Interpreter interpreter;

        public StmtVisitor(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public object VisitAssignment(Assignment stmt)
        {
            var value = interpreter.Evaluate(stmt.Value);
            interpreter.SetVariable(stmt.Name, value);
            return null;
        }        public object VisitSpawn(Spawn stmt)
        {
            int x = Convert.ToInt32(interpreter.Evaluate(stmt.X));
            int y = Convert.ToInt32(interpreter.Evaluate(stmt.Y));
            interpreter.Spawn(x, y);
            return null;
        }

        public object VisitColor(Color stmt)
        {
            string color = (string)interpreter.Evaluate(stmt.ColorName);
            interpreter.SetBrushColor(color);
            return null;
        }        public object VisitSize(Size stmt)
        {
            int size = Convert.ToInt32(interpreter.Evaluate(stmt.SizeValue));
            interpreter.SetBrushSize(size);
            return null;
        }public object VisitDrawLine(DrawLine stmt)
        {
            int dx = Convert.ToInt32(interpreter.Evaluate(stmt.DirX));
            int dy = Convert.ToInt32(interpreter.Evaluate(stmt.DirY));
            int dist = Convert.ToInt32(interpreter.Evaluate(stmt.Distance));
            interpreter.DrawLine(dx, dy, dist);
            return null;
        }

        public object VisitDrawCircle(DrawCircle stmt)
        {
            int dx = Convert.ToInt32(interpreter.Evaluate(stmt.DirX));
            int dy = Convert.ToInt32(interpreter.Evaluate(stmt.DirY));
            int radius = Convert.ToInt32(interpreter.Evaluate(stmt.Radius));
            interpreter.DrawCircle(dx, dy, radius);
            return null;
        }        public object VisitDrawRectangle(DrawRectangle stmt)
        {
            int dx = Convert.ToInt32(interpreter.Evaluate(stmt.DirX));
            int dy = Convert.ToInt32(interpreter.Evaluate(stmt.DirY));
            int dist = Convert.ToInt32(interpreter.Evaluate(stmt.Distance));
            int width = Convert.ToInt32(interpreter.Evaluate(stmt.Width));
            int height = Convert.ToInt32(interpreter.Evaluate(stmt.Height));
            interpreter.DrawRectangle(dx, dy, dist, width, height);
            return null;
        }

        public object VisitFill(Fill stmt)
        {
            interpreter.Fill();
            return null;
        }        public object VisitGoto(Goto stmt)
        {
            bool condition = (bool)interpreter.Evaluate(stmt.Condition);
            if (condition)
            {
                throw new GotoException(stmt.LabelName);
            }
            return null;
        }

        public object VisitLabel(Label stmt)
        {
            return null;
        }
    }
}