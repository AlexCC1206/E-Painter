using System.Linq.Expressions;

namespace EPainter.Core
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);

        public interface IStmtVisitor<T>
        {
            T VisitAssignment(Assignment stmt);
            T VisitSpawn(Spawn stmt);
            T VisitColor(Color stmt);
            T VisitSize(Size stmt);
            T VisitDrawLine(DrawLine stmt);
            T VisitDrawCircle(DrawCircle stmt);
            T VisitDrawRectangle(DrawRectangle stmt);
            T VisitFill(Fill stmt);
            T VisitGoto(Goto stmt);
            T VisitLabel(Label stmt);
        }

        public class Assignment : Stmt
        {
            public string Name { get; }
            public Expr Value { get; }


            public Assignment(string name, Expr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitAssignment(this);
            }
        }

        public class Spawn : Stmt
        {
            public Expr X { get; }
            public Expr Y { get; }


            public Spawn(Expr x, Expr y)
            {
                X = x;
                Y = y;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitSpawn(this);
            }
        }

        public class Color : Stmt
        {
            public Expr ColorName { get; }


            public Color(Expr colorName)
            {
                ColorName = colorName;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitColor(this);
            }
        }

        public class Size : Stmt
        {
            public Expr SizeValue { get; }


            public Size(Expr sizeValue)
            {
                SizeValue = sizeValue;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitSize(this);
            }
        }

        public class DrawLine : Stmt
        {
            public Expr DirX { get; }
            public Expr DirY { get; }
            public Expr Distance { get; }


            public DrawLine(Expr dirX, Expr dirY, Expr distance)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitDrawLine(this);
            }
        }

        public class DrawCircle : Stmt
        {
            public Expr DirX { get; }
            public Expr DirY { get; }
            public Expr Radius { get; }


            public DrawCircle(Expr dirX, Expr dirY, Expr radius)
            {
                DirX = dirX;
                DirY = dirY;
                Radius = radius;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitDrawCircle(this);
            }
        }

        public class DrawRectangle : Stmt
        {
            public Expr DirX { get; }
            public Expr DirY { get; }
            public Expr Distance { get; }
            public Expr Width { get; }
            public Expr Height { get; }

            public DrawRectangle(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
                Width = width;
                Height = height;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitDrawRectangle(this);
            }
        }

        public class Fill : Stmt
        {
            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitFill(this);
            }
        }

        public class Goto : Stmt
        {
            public string LabelName { get; }
            public Expr Condition { get; }


            public Goto(string label, Expr condition)
            {
                LabelName = label;
                Condition = condition;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitGoto(this);
            }
        }

        public class Label : Stmt
        {
            public string Name { get; }


            public Label(string name)
            {
                Name = name;
            }

            public override T Accept<T>(IStmtVisitor<T> visitor)
            {
                return visitor.VisitLabel(this);
            }
        }
    }
}