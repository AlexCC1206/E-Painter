using System;
using System.Collections.Generic;
using Godot;

namespace EPainter
{
    public class Interpreter : IVisitor<object>
    {
        public object VisitBinaryExpr(Binary binary)
        {
            object left = evaluate(binary.left);
            object right = evaluate(binary.rigth);

            switch (binary.Operator.type)
            {
                case TokenType.MIN:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.SUM:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left + (double)right;
                case TokenType.DIV:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.MULT:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.MOD :
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left % (double)right;
                case TokenType.POW : 
                    checkNumberOperand(binary.Operator, left, right);
                    return Math.Pow((double)left, (double)right);

                case TokenType.GREATER:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperand(binary.Operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.EQUAL_EQUAL:
                    checkNumberOperand(binary.Operator, left, right);
                    return isEqual(left, right);

                case TokenType.AND :
                    return IsTruty(left) && IsTruty(right);
                case TokenType.OR :
                    return IsTruty(left) || IsTruty(right);
            }

            throw new RuntimeError(binary.Operator, "Operator not supported");
        }

        public object VisitFunctionCallExpr(FunctionCall functionCall)
        {
            var args = new List<object>();
            foreach (var arg in functionCall.Arguments)
            {
                args.Add(evaluate(arg));
            }

            
            
            throw new NotImplementedException();
        }

        public object VisitGroupingExpr(Grouping grouping)
        {
            return evaluate(grouping.Expr);
        }

        public object VisitLiteralExpr(Literal literal)
        {
            return literal.Value;
        }

        public object VisitUnaryExpr(Unary unary)
        {
            object right = evaluate(unary.right);

            switch (unary.Operator.type)
            {
                case TokenType.MIN:
                    checkNumberOperand(unary.Operator, right);
                    return -(double)right;
            }

            throw new RuntimeError(unary.Operator, "Operator unary not supported");
        }

        public object VisitVariableExpr(Variable variable)
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
            if (left is double && right is double) return;

            throw new RuntimeError(Operator, "Operands must be a numbers");
        }

        private bool isEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        private void checkNumberOperand(Token Operator, object right)
        {
            if (right is double) return;

            throw new RuntimeError(Operator, "Operand must be a number");
        }

        private object evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        internal void Interpret(Expr expr)
        {
            try
            {
                object Value = evaluate(expr);
                Console.WriteLine($"{stringify(Value)}");
            }
            catch (RuntimeError error)
            {
                EPainter.runtimeError(error);
            }
        }

        private string stringify(object value)
        {
            if (value is double)
            {
                string text = value.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return value.ToString();
        }
    }
}