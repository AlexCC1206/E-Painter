namespace EPainter
{
    public abstract class Command
    {
        public abstract T Accept<T>(ICommandVisitor<T> visitor);

        public interface ICommandVisitor<T>
        {
            T VisitSpawnCommand(Spawn spawn);
            T VisitColorCommand(Color color);
            T VisitSizeCommand(Size size);
            T VisitDrawLineCommand(DrawLine drawline);
            T VisitDrawCircleCommand(DrawCircle drawCircle);
            T VisitDrawRectangleCommand(DrawRectangle drawRectangle);
            T VisitFillCommand(Fill fill);
            T VisitAssignmentCommand(Assignment assignment);
            T VisitLabelCommand(Label label);
            T VisitGotoCommand(Goto @goto);
        }

        public class Spawn : Command
        {
            public Expr X;
            public Expr Y;

            public Spawn(Expr x, Expr y)
            {
                X = x;
                Y = y;
            }

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitSpawnCommand(this);
            }
        }

        public class Color : Command
        {
            public Expr ColorName;

            public Color(Expr colorName)
            {
                ColorName = colorName;
            }

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitColorCommand(this);
            }
        }

        public class Size : Command
        {
            public Expr SizeValue;

            public Size(Expr sizeValue)
            {
                SizeValue = sizeValue;
            }

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitSizeCommand(this);
            }
        }

        public class DrawLine : Command
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

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitDrawLineCommand(this);
            }
        }

        public class DrawCircle : Command
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

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitDrawCircleCommand(this);
            }
        }

        public class DrawRectangle : Command
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

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitDrawRectangleCommand(this);
            }
        }

        public class Fill : Command
        {
            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitFillCommand(this);
            }
        }

        public class Assignment : Command
        {
            public Token Name;
            public Expr Value;

            public Assignment(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitAssignmentCommand(this);
            }
        }

        public class Label : Command
        {
            public Token Name;

            public Label(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitLabelCommand(this);
            }
        }

        public class Goto : Command
        {
            public Token label;
            public Expr Condition;

            public Goto(Token label, Expr condition)
            {
                this.label = label;
                Condition = condition;
            }

            public override T Accept<T>(ICommandVisitor<T> visitor)
            {
                return visitor.VisitGotoCommand(this);
            }
        }
    }
}