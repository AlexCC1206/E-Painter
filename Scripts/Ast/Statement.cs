namespace EPainter
{
    public abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);

        public interface IStatementVisitor<T>
        {
            T VisitSpawnStatement(Spawn spawn);
            T VisitColorStatement(Color color);
            T VisitSizeStatement(Size size);
            T VisitDrawLineStatement(DrawLine drawline);
            T VisitDrawCircleStatement(DrawCircle drawCircle);
            T VisitDrawRectangleStatement(DrawRectangle drawRectangle);
            T VisitFillStatement(Fill fill);
            T VisitAssignmentStatement(Assignment assignment);
            T VisitLabelStatement(Label label);
            T VisitGotoStatement(Goto @goto);
        }

        public class Spawn : Statement
        {
            public Expr X;
            public Expr Y;

            public Spawn(Expr x, Expr y)
            {
                X = x;
                Y = y;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitSpawnStatement(this);
            }
        }

        public class Color : Statement
        {
            public Expr ColorName;

            public Color(Expr colorName)
            {
                ColorName = colorName;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitColorStatement(this);
            }
        }

        public class Size : Statement
        {
            public Expr SizeValue;

            public Size(Expr sizeValue)
            {
                SizeValue = sizeValue;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitSizeStatement(this);
            }
        }

        public class DrawLine : Statement
        {
            public Expr DirX;
            public Expr DirY;
            public Expr Distance;

            public DrawLine(Expr dirX, Expr dirY, Expr distance)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitDrawLineStatement(this);
            }
        }

        public class DrawCircle : Statement
        {
            public Expr DirX;
            public Expr DirY;
            public Expr Radius;
            public DrawCircle(Expr dirX, Expr dirY, Expr radius)
            {
                DirX = dirX;
                DirY = dirY;
                Radius = radius;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitDrawCircleStatement(this);
            }
        }

        public class DrawRectangle : Statement
        {
            public Expr DirX;
            public Expr DirY;
            public Expr Distance;
            public Expr Width;
            public Expr Height;

            public DrawRectangle(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
            {
                DirX = dirX;
                DirY = dirY;
                Distance = distance;
                Width = width;
                Height = height;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitDrawRectangleStatement(this);
            }
        }

        public class Fill : Statement
        {
            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitFillStatement(this);
            }
        }

        public class Assignment : Statement
        {
            public Token Name;
            public Expr Value;

            public Assignment(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitAssignmentStatement(this);
            }
        }

        public class Label : Statement
        {
            public Token Name;

            public Label(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitLabelStatement(this);
            }
        }

        public class Goto : Statement
        {
            public Token label;
            public Expr Condition;

            public Goto(Token label, Expr condition)
            {
                this.label = label;
                Condition = condition;
            }

            public override T Accept<T>(IStatementVisitor<T> visitor)
            {
                return visitor.VisitGotoStatement(this);
            }
        }
    }
}