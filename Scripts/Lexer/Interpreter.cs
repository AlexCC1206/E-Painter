using System;
using System.Collections.Generic;
using Godot;

namespace EPainter
{
    public class Interpreter : Expr.IVisitor<object>, Command.ICommandVisitor<bool>
    {
        public object VisitBinaryExpr(Expr.Binary binary)
        {
            object left = Evaluate(binary.left);
            object right = Evaluate(binary.rigth);

            switch (binary.Operator.type)
            {
                case TokenType.MIN:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left - (int)right;
                case TokenType.SUM:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left + (int)right;
                case TokenType.DIV:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left / (int)right;
                case TokenType.MULT:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left * (int)right;
                case TokenType.MOD:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left % (int)right;
                case TokenType.POW:
                    checkNumberOperand(binary.Operator, left, right);
                    return Math.Pow((int)left, (int)right);

                case TokenType.GREATER:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left > (int)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left >= (int)right;
                case TokenType.LESS:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left < (int)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperand(binary.Operator, left, right);
                    return (int)left <= (int)right;
                case TokenType.EQUAL_EQUAL:
                    checkNumberOperand(binary.Operator, left, right);
                    return isEqual(left, right);

                case TokenType.AND:
                    return IsTruty(left) && IsTruty(right);
                case TokenType.OR:
                    return IsTruty(left) || IsTruty(right);
            }

            throw new RuntimeError(binary.Operator, "Operator not supported");
        }

        public object VisitFunctionCallExpr(Expr.FunctionCall functionCall)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Expr.Grouping grouping)
        {
            return Evaluate(grouping.Expr);
        }

        public object VisitLiteralExpr(Expr.Literal literal)
        {
            return literal.Value;
        }
        
        public object VisitVariableExpr(Expr.Variable variable)
        {
            throw new NotImplementedException();
        }

        private bool IsTruty(object obj)
        {
            if (obj is bool b) return b;
            return false;
        }

        private void checkNumberOperand(Token Operator, object left, object right)
        {
            if (left is int && right is int) return;

            throw new RuntimeError(Operator, "Operands must be a numbers");
        }

        private bool isEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        public bool VisitSpawnCommand(Command.Spawn spawn)
        {
            var x = Evaluate(spawn.X);
            var y = Evaluate(spawn.Y);


            throw new NotImplementedException();
        }

        public bool VisitColorCommand(Command.Color color)
        {
            throw new NotImplementedException();
        }

        public bool VisitSizeCommand(Command.Size size)
        {
            throw new NotImplementedException();
        }

        public bool VisitDrawLineCommand(Command.DrawLine drawline)
        {
            throw new NotImplementedException();
        }

        public bool VisitDrawCircleCommand(Command.DrawCircle drawCircle)
        {
            throw new NotImplementedException();
        }

        public bool VisitDrawRectangleCommand(Command.DrawRectangle drawRectangle)
        {
            throw new NotImplementedException();
        }

        public bool VisitFillCommand(Command.Fill fill)
        {
            throw new NotImplementedException();
        }

        public bool VisitAssignmentCommand(Command.Assignment assignment)
        {
            throw new NotImplementedException();
        }

        public bool VisitLabelCommand(Command.Label label)
        {
            throw new NotImplementedException();
        }

        public bool VisitGotoCommand(Command.Goto @goto)
        {
            throw new NotImplementedException();
        }
        /*
internal void Interpret(Expr expr)
{
  try
  {
      object Value = Evaluate(expr);
      Console.WriteLine($"{stringify(Value)}");
  }
  catch (RuntimeError error)
  {
      EPainter.runtimeError(error);
  }
}*/
        /*
                internal void Interpret(List<Stmt> stmts)
                {
                    try
                    {
                        foreach (var stmt in stmts)
                        {
                            execute(stmt);
                        }
                    }
                    catch (RuntimeError error)
                    {
                        EPainter.runtimeError(error);
                    }
                }

                private void execute(Stmt stmt)
                {
                    stmt.Accept(this);
                }

                private string stringify(object value)
                {
                    if (value is int)
                    {
                        string text = value.ToString();
                        if (text.EndsWith(".0"))
                        {
                            text = text.Substring(0, text.Length - 2);
                        }
                        return text;
                    }

                    return value.ToString();
                }*/
    }
}