using System.Linq;
using System.Text;
using EPainter.Core;
using static EPainter.Core.Expr;
using static EPainter.Core.Stmt;

public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string Print(Stmt stmt)
    {
        return stmt.Accept(this);
    }

    public string VisitBinaryExpr(Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Literal expr)
    {
        return expr.Value?.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string VisitVariableExpr(Variable expr)
    {
        return expr.Name.Lexeme;
    }

    public string VisitFunctionCallExpr(FunctionCall expr)
    {
        var args = string.Join(", ", expr.Arguments.Select(a => a.Accept(this)));
        return $"{expr.Name.Lexeme}({args})";
    }

    public string VisitBlockStmt(Block stmt)
    {
        var builder = new StringBuilder();
        builder.AppendLine("{");
        foreach (var statement in stmt.Statements)
        {
            builder.AppendLine(statement.Accept(this));
        }
        builder.Append("}");
        return builder.ToString();
    }

    public string VisitCommandStmt(Command stmt)
    {
        var args = string.Join(", ", stmt.Arguments.Select(a => a.Accept(this)));
        return $"{stmt.Name.Lexeme}({args}):";
    }

    public string VisitExpressionStmt(Expression stmt)
    {
        return $"{stmt.Expr.Accept(this)}:";
    }

    public string VisitVarStmt(Var stmt)
    {
        return $"{stmt.Name.Lexeme} <- {stmt.Initializer.Accept(this)}:";
    }

    public string VisitLabelStmt(Label stmt)
    {
        return $"{stmt.Name.Lexeme}:";
    }

    public string VisitGotoStmt(Goto stmt)
    {
        return $"GoTo [{stmt.Label.Lexeme}] ({stmt.Condition.Accept(this)}):";
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder();
        builder.Append("(").Append(name);
        foreach (var expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }
}