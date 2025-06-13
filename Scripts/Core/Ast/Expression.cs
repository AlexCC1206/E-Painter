using System.Collections.Generic;

namespace EPainter.Core
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);


        public interface IExprVisitor<T>
        {
            T VisitLiteral(Literal expr);
            T VisitVariable(Variable expr);
            T VisitUnary(Unary expr);
            T VisitBinary(Binary expr);
            T VisitGrouping(Grouping expr);
            T VisitCall(Call expr);
        }

        public class Literal : Expr
        {
            public object Value { get; }

            public Literal(object value)
            {
                Value = value;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitLiteral(this);
            }
        }

        public class Variable : Expr
        {
            public string Name { get; }

            public Variable(string name)
            {
                Name = name;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitVariable(this);
            }
        }

        public class Binary : Expr
        {
            public Expr Left { get; }
            public Token Op { get; }
            public Expr Right { get; }

            public Binary(Expr left, Token op, Expr right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitBinary(this);
            }
        }

        public class Unary : Expr
        {
            public Token Op { get; }
            public Expr Right { get; }


            public Unary(Token op, Expr right)
            {
                Op = op;
                Right = right;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitUnary(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr Expression { get; }

            public Grouping(Expr expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitGrouping(this);
            }
        }
        
        public class Call : Expr
        {
            public string FunctionName { get; }
            public List<Expr> Arguments { get; }

            public Call(string functionName, List<Expr> args)
            {
                FunctionName = functionName;
                Arguments = args;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.VisitCall(this);
            }
        }
    }
}


