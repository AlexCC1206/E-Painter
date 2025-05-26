using System.Text;

namespace EPainter
{
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary binary)
        {
            return Parenthesize(binary.Operator.lexeme, binary.left, binary.rigth);
        }

        public string VisitGroupingExpr(Grouping grouping)
        {
            return Parenthesize("group", grouping.Expr);
        }

        public string VisitLiteralExpr(Literal literal)
        {
            if(literal.Value is string str)
            {
                return $"\"{str}\"";
            }

            if(literal.Value == null)
            {
                return "null";
            }

            return literal.Value.ToString();
        }

        public string VisitUnaryExpr(Unary unary)
        {
            return Parenthesize(unary.Operator.lexeme, unary.right);
        }

        public string VisitVariableExpr(Variable variable)
        {
            return variable.Name;
        }

        public string VisitFunctionCallExpr(FunctionCall functionCall)
        {
            var builder = new StringBuilder();
            builder.Append($"{functionCall.Name}(");

            for (var i = 0; i < functionCall.Arguments.Count; i++)
            {
               if(i > 0) builder.Append(", ");
               builder.Append(functionCall.Arguments[i].Accept(this)); 
            }

            builder.Append(")");
            return builder.ToString();
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