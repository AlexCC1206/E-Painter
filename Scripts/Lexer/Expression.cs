using System.Collections.Generic;

namespace EPainter
{
    public abstract class Expression
    {
        public abstract T Aceptar<T>(IVisitor<T> visitor);
    }

    public interface IVisitor<T>
    {
        T visitBinary(Binary binary);
        T visitUnary(Unary unary);
        T visitLiteral(Literal literal);
        T visitFunctionCall(FunctionCall functionCall);
        T visitAssign(Assign assign);
        T visitGrouping(Grouping grouping);
        

        // T visitVariable(Variable variable);
        // T visitBoolean(BooleanLiteral boolean);
        // T visitNumber(NumberLiteral number);
    }

    public class Binary : Expression
    {
        public Expression left;
        public Token Operator;
        public Expression rigth;

        public Binary(Expression left, Token Operator, Expression rigth)
        {
            this.left = left;
            this.Operator = Operator;
            this.rigth = rigth;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitBinary(this);
        }
    }

    public class Unary : Expression
    {
        public Token Operator;
        public Expression right;

        public Unary(Token Operator, Expression right)
        {
            this.Operator = Operator;
            this.right = right;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitUnary(this);
        }
    }

    public class Literal : Expression
    {
        public object Value;

        public Literal(object Value)
        {
            this.Value = Value;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitLiteral(this);
        }
    }
/*
    public class NumberLiteral : Expression
    {
        public int Value;

        public NumberLiteral(int Value)
        {
            this.Value = Value;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitNumber(this);
        }
    }

    public class BooleanLiteral : Expression
    {
        public bool Value;

        public BooleanLiteral(bool Value)
        {
            this.Value = Value;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitBoolean(this);
        }
    }

    public class Variable : Expression
    {
        public string Name;

        public Variable(string Name)
        {
            this.Name = Name;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitVariable(this);
        }
    }
*/
    public class Grouping : Expression
    {
        public Expression expression;

        public Grouping(Expression expression)
        {
            this.expression = expression;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitGrouping(this);
        }
    }

    public class FunctionCall : Expression
    {
        public string Name;
        public List<Expression> Arguments;

        public FunctionCall(string name, List<Expression> args)
        {
            Name = name;
            Arguments = args;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitFunctionCall(this);
        }
    }

    public class Assign : Expression
    {
        public string Value;

        public Assign(string Value)
        {
            this.Value = Value;
        }

        public override T Aceptar<T>(IVisitor<T> visitor)
        {
            return visitor.visitAssign(this);
        }
    }
}


    