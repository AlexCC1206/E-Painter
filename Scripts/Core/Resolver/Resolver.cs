using System;
using System.Collections.Generic;
using System.Linq;
using EPainter.Core;
using static EPainter.Core.Expr;
using static EPainter.Core.Stmt;

namespace EPainter
{
    public class Resolver : IStmtVisitor<object>, IExprVisitor<object>
    {
        private Interpreter Interpreter;
        private Stack<Dictionary<string, bool>> Scopes = new();

        public Resolver(Interpreter interpreter)
        {
            Interpreter = interpreter;
        }

        public void Resolve(List<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                Resolve(stmt);
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

        private void BeginScope()
        {
            Scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            Scopes.Pop();
        }

        private void Declare(string name)
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var scope = Scopes.Peek();
            if (scope.ContainsKey(name))
            {
                throw new Exception($"Variable '{name}' already declared in this scope.");
            }

            scope[name] = false;
        }

        private void Define(string name)
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var scope = Scopes.Peek();
            scope[name] = true;
        }

        private void ResolveLocal(string name, Expr expr)
        {
            for (int i = 0; i < Scopes.Count; i++)
            {
                if (Scopes.ElementAt(i).ContainsKey(name))
                {
                    return;
                }
            }

            throw new Exception($"Undefined variable '{name}'.");
        }

        public object VisitAssignment(Assignment stmt)
        {
            Resolve(stmt.Value);
            Declare(stmt.Name);
            Define(stmt.Name);
            return null;
        }

        public object VisitBinary(Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object VisitCall(Call expr)
        {
            foreach (var arg in expr.Arguments)
            {
                Resolve(arg);
            }

            return null;
        }

        public object VisitColor(Color stmt)
        {
            Resolve(stmt.ColorName);
            return null;
        }

        public object VisitDrawCircle(DrawCircle stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Radius);
            return null;
        }

        public object VisitDrawLine(DrawLine stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Distance);
            return null;
        }

        public object VisitDrawRectangle(DrawRectangle stmt)
        {
            Resolve(stmt.DirX);
            Resolve(stmt.DirY);
            Resolve(stmt.Distance);
            Resolve(stmt.Width);
            Resolve(stmt.Height);
            return null;
        }

        public object VisitFill(Fill stmt)
        {
            return null;
        }

        public object VisitGoto(Goto stmt)
        {
            if (!Interpreter.Labels.ContainsKey(stmt.LabelName))
            {
                throw new Exception($"GoTo to undeclared label '{stmt.LabelName}'.");
            }

            Resolve(stmt.Condition);
            return null;
        }

        public object VisitGrouping(Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object VisitLabel(Label stmt)
        {
            return null;
        }

        public object VisitLiteral(Literal expr)
        {
            return null;
        }

        public object VisitSize(Size stmt)
        {
            Resolve(stmt.SizeValue);
            return null;
        }

        public object VisitSpawn(Spawn stmt)
        {
            Resolve(stmt.X);
            Resolve(stmt.Y);
            return null;
        }

        public object VisitUnary(Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        public object VisitVariable(Variable expr)
        {
            if (Scopes.Count > 0 && Scopes.Peek().ContainsKey(expr.Name))
            {
                var isInitialized = Scopes.Peek()[expr.Name];
                if (!isInitialized)
                {
                    throw new Exception($"Variable '{expr.Name}' used before assignment.");
                }
            }

            ResolveLocal(expr.Name, expr);
            return null;
        }
    }
}