using System.Collections.Generic;

namespace EPainter
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitBinaryExpr(Binary binary);
            T VisitGroupingExpr(Grouping grouping);
            T VisitLiteralExpr(Literal literal);
            T VisitVariableExpr(Variable variable);
            T VisitFunctionCallExpr(FunctionCall functionCall);
        }

        public class Binary : Expr
        {
            public Expr left;
            public Token Operator;
            public Expr rigth;

            public Binary(Expr left, Token Operator, Expr rigth)
            {
                this.left = left;
                this.Operator = Operator;
                this.rigth = rigth;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr Expr;

            public Grouping(Expr Expr)
            {
                this.Expr = Expr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public object Value;

            public Literal(object Value)
            {
                this.Value = Value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }
        
        public class Variable : Expr
        {
            public Token Name;

            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        public class FunctionCall : Expr
        {
            public Token Name;
            public List<Expr> Arguments;

            public FunctionCall(Token name, List<Expr> args)
            {
                Name = name;
                Arguments = args;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFunctionCallExpr(this);
            }
        }
    }
}


