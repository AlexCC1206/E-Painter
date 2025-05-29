using System.Collections.Generic;
using System.Text;

namespace EPainter
{
    public class AstPrinter : Expr.IVisitor<string>, Command.ICommandVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Print(Command command)
        {
            return command.Accept(this);
        }

        public string PrintProgram(List<Command> commands)
        {
            var builder = new StringBuilder();
            foreach (var command in commands)
            {
                builder.AppendLine(command.Accept(this));
            }

            return builder.ToString();
        }

        public string VisitBinaryExpr(Expr.Binary binary)
        {
            return Parenthesize(binary.Operator.lexeme, binary.left, binary.rigth);
        }

        public string VisitGroupingExpr(Expr.Grouping grouping)
        {
            return Parenthesize("group", grouping.Expr);
        }

        public string VisitLiteralExpr(Expr.Literal literal)
        {
            if (literal.Value is string str)
            {
                return $"\"{str}\"";
            }

            if (literal.Value == null)
            {
                return "null";
            }

            return literal.Value.ToString();
        }

        public string VisitVariableExpr(Expr.Variable variable)
        {
            return variable.Name.lexeme;
        }

        public string VisitFunctionCallExpr(Expr.FunctionCall functionCall)
        {
            var builder = new StringBuilder();
            builder.Append($"{functionCall.Name}(");

            for (var i = 0; i < functionCall.Arguments.Count; i++)
            {
                if (i > 0) builder.Append(", ");
                builder.Append(functionCall.Arguments[i].Accept(this));
            }

            builder.Append(")");
            return builder.ToString();
        }

        public string VisitSpawnCommand(Command.Spawn spawn)
        {
            return $"Spawn({spawn.X.Accept(this)}, {spawn.Y.Accept(this)})";
        }

        public string VisitColorCommand(Command.Color color)
        {
            return $"Color({color.ColorName.Accept(this)})";
        }

        public string VisitSizeCommand(Command.Size size)
        {
            return $"Size({size.SizeValue.Accept(this)})";
        }

        public string VisitDrawLineCommand(Command.DrawLine drawline)
        {
            return $"DrawLine({drawline.DirX.Accept(this)}, {drawline.DirY.Accept(this)}, {drawline.Distance.Accept(this)})";
        }

        public string VisitDrawCircleCommand(Command.DrawCircle drawCircle)
        {
            return $"DrawCircle({drawCircle.DirX.Accept(this)}, {drawCircle.DirY.Accept(this)}, {drawCircle.Radius.Accept(this)})";
        }

        public string VisitDrawRectangleCommand(Command.DrawRectangle drawRectangle)
        {
            return $"DrawRectangle({drawRectangle.DirX.Accept(this)}, {drawRectangle.DirY.Accept(this)}, {drawRectangle.Distance.Accept(this)}, {drawRectangle.Width.Accept(this)}, {drawRectangle.Height.Accept(this)})";
        }

        public string VisitFillCommand(Command.Fill fill)
        {
            return $"Fill()";
        }

        public string VisitAssignmentCommand(Command.Assignment assignment)
        {
            return $"{assignment.Name.lexeme} <- {assignment.Value.Accept(this)}";
        }

        public string VisitLabelCommand(Command.Label label)
        {
            return $"{label.Name.lexeme}:";
        }

        public string VisitGotoCommand(Command.Goto @goto)
        {
            return $"Goto [{@goto.label.lexeme}] ({@goto.Condition.Accept(this)})";
        }

        public string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();
            builder.Append($"({name}");

            foreach (var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }

            builder.Append(")");
            return builder.ToString();
        }


    }
}