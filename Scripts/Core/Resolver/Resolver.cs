using System.Collections.Generic;
using System.Linq;

namespace EPainter.Core
{
    public class Resolver : Expr.IVisitor<Void>, Stmt.IVisitor<Void>
    {
        public Interpreter interpreter;
        private Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        private enum FunctionType
        {
            NONE,
            FUNCTION
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }
/*
        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0)
            {
                return;
            }

            Dictionary<string, bool> scope = scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                ErrorReporter.Error(name, "Variable with this name already declared in this scope.");
            }

            scope[name.Lexeme] = false;
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0)
            {
                return;
            }
            scopes.Peek()[name.Lexeme] = true;
        }*/

        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, scopes.Count - 1 - i);
                    return;
                }
            }
        }

        public Void VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public Void VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public Void VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public Void VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        public Void VisitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count > 0)
            {
                Dictionary<string, bool> scope = scopes.Peek();
                if (scope.TryGetValue(expr.Name.Lexeme, out bool defined) && !defined)
                {
                    ErrorReporter.Error(expr.Name, "Cannot read local variable in its own initializer.");
                }
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public Void VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public Void VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public Void VisitSpawnStmt(Stmt.Spawn stmt)
        {
            Resolve(stmt.X);
            Resolve(stmt.Y);
            return null;
        }

        public Void VisitColorStmt(Stmt.Color stmt)
        {
            return null;
        }

        public Void VisitSizeStmt(Stmt.Size stmt)
        {
            Resolve(stmt.SizeValue);
            return null;
        }

        public Void VisitDrawLineStmt(Stmt.DrawLine stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Distance);
            return null;
        }

        public Void VisitDrawCircleStmt(Stmt.DrawCircle stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Radius);
            return null;
        }

        public Void VisitDrawRectangleStmt(Stmt.DrawRectangle stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Distance);
            Resolve(stmt.Width);
            Resolve(stmt.Height);
            return null;
        }

        public Void VisitFillStmt(Stmt.Fill stmt)
        {
            return null;
        }

        public Void VisitGotoStmt(Stmt.Goto stmt)
        {
            Resolve(stmt.Condition);
            return null;
        }

        public Void VisitAssignmentStmt(Stmt.Assignment stmt)
        {
            Resolve(stmt.Value);
            ResolveLocal(stmt.Value, stmt.Name);
            return null;
        }

        public Void VisitLabelStmt(Stmt.Label stmt)
        {
            return null;
        }
    }

    public class Void
    {

    }
}